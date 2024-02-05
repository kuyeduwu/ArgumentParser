using System.Diagnostics;
using System.Reflection;
using System.Text;
using ArgumentParser.Attributes;

namespace ArgumentParser;

public class ArgumentParser<T>
{
    private readonly ArgumentParsingConfig _config;

    public ArgumentParser(ArgumentParsingConfig config)
    {
        this._config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public ArgumentParsingResult<T> TryParse(string[] args)
    {
        var fields = GetArgumentedFields();

        if (!fields.Any())
            return new ArgumentParsingResult<T>(false, default, $"No parsable fields defined on: {typeof(T)}");

        if (!IsNamesIdentical())
            return new ArgumentParsingResult<T>(false, default, "Duplicate short/long argument name found");

        var target = Activator.CreateInstance<T>();
        if (target == null) throw new Exception($"Cannot create an instance of {typeof(T)}");
        
        var parsedIndex = new List<int>();
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<BaseArgumentAttribute>();
            if (attr == null) continue;
            try
            {
                var indexes = attr.SetFieldValue(args, target, field, this._config);
                parsedIndex.AddRange(indexes.Where(r => r != -1).ToList());
            }
            catch (Exception ex)
            {
                var result = new ArgumentParsingResult<T>(false, default, ex.Message);
                return result;
            }
        }

        var argIndexes = Enumerable.Range(0, args.Length).ToList();
        var unParsedIndex = argIndexes.Where(r => !parsedIndex.Contains(r));

        if (unParsedIndex != null && unParsedIndex.Any())
        {
            return new ArgumentParsingResult<T>(false, default,
                $"Unknown argument {args[unParsedIndex.First()]} at position {unParsedIndex.First()}");
        }

        return new ArgumentParsingResult<T>(true, target, null);
    }
    
    /// <summary>
    /// Get the help message which can be print to the console
    /// </summary>
    /// <returns></returns>
    public string GetHelpMessage()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(assemblyName);
        sb.AppendLine("-".PadRight(20, '-'));
        sb.AppendLine("Arguments:");
        foreach (var field in GetArgumentedFields())
        {
            var attr = field.GetCustomAttribute<BaseArgumentAttribute>() as BaseArgumentAttribute;

            var shortName = string.IsNullOrWhiteSpace(attr.ShortName)
                ? string.Empty
                : (string.IsNullOrWhiteSpace(_config.ShortNamePrefix)
                    ? attr.ShortName
                    : $"{_config.ShortNamePrefix}{attr.ShortName}");
            var longName = string.IsNullOrWhiteSpace(attr.LongName)
                ? string.Empty
                : (string.IsNullOrWhiteSpace(_config.LongNamePrefix)
                    ? attr.LongName
                    : $"{_config.LongNamePrefix}{attr.LongName}");
            var required = attr.IsRequired ? "Required" : string.Empty;

            sb.AppendLine($"{shortName.PadRight(5)}{longName.PadRight(15)}{required.PadRight(10)}{attr.HelpText}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Detect whether each of the defined short/long name is identical.
    /// </summary>
    /// <returns></returns>
    private bool IsNamesIdentical()
    {
        var fields = GetArgumentedFields();
        var definedArguments = fields.Select(r => r.GetCustomAttribute<BaseArgumentAttribute>());
        var baseArgumentAttributes = definedArguments as BaseArgumentAttribute[] ?? definedArguments.ToArray();
        var names = baseArgumentAttributes.Select(r => (r as BaseArgumentAttribute)?.ShortName).ToList();
        var longNames = baseArgumentAttributes.Select(r => (r as BaseArgumentAttribute)?.LongName).ToList();
        
        names.AddRange(longNames);

        names = names.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();

        return names.Count == names.Distinct().Count();
    }
    
    private FieldInfo[] GetArgumentedFields()
    {
        var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        return fields.Where(f => f.GetCustomAttributes<BaseArgumentAttribute>().Any()).ToArray();
    }
}