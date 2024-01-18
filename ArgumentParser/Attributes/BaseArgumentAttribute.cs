using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgumentParser.Attributes;

public abstract class BaseArgumentAttribute : Attribute
{
    /// <summary>
    /// The short name of the argument, optional if long name is provided.
    /// </summary>
    public string? ShortName { get; init; }
    
    /// <summary>
    /// The long name of the argument, optional if short name is provided.
    /// </summary>
    public string? LongName { get; init; }
    
    /// <summary>
    /// Whether this argument is required or not, default false.
    /// </summary>
    public bool IsRequired { get; }
    
    /// <summary>
    /// The help text, mandatory.
    /// </summary>
    public string HelpText { get; }

    protected BaseArgumentAttribute(string? shortName, string? longName, bool required, string help)
    {
        if (string.IsNullOrWhiteSpace(help)) throw new ArgumentNullException(nameof(help));
        
        this.ShortName = shortName;
        this.LongName = longName;
        this.IsRequired = required; 
        this.HelpText = help;

        if (string.IsNullOrWhiteSpace(this.ShortName) && string.IsNullOrWhiteSpace(this.LongName))
            throw new ArgumentException($"{nameof(ShortName)} and {nameof(LongName)} cannot be null at the same time.");
    }

    /// <summary>
    /// Based on the user provided arguments, fill the field value of the argument object.
    /// </summary>
    /// <param name="args">The arguments received from command line.</param>
    /// <param name="instance">The instance of the argument object.</param>
    /// <param name="field">The field to set value.</param>
    /// <param name="config">The parser configuration</param>
    /// <returns></returns>
    public abstract IEnumerable<int> SetFieldValue(string[] args, object instance, FieldInfo field, ArgumentParsingConfig config);

    /// <summary>
    /// Get the index of the current argument field, within all the command line arguments.
    /// </summary>
    /// <param name="args">The arguments received from command line.</param>
    /// <param name="config">The parser configuration</param>
    /// <returns></returns>
    internal int GetIndexFromCommandLine(IEnumerable<string> args, ArgumentParsingConfig config)
    {
        var allowedNames = new List<string>();

        var shortInput = string.IsNullOrWhiteSpace(this.ShortName)
            ? null
            : (string.IsNullOrWhiteSpace(config.ShortNamePrefix)
                ? this.ShortName
                : $"{config.ShortNamePrefix}{this.ShortName}");
        var longInput = string.IsNullOrWhiteSpace(this.LongName)
            ? null
            : (string.IsNullOrWhiteSpace(config.LongNamePrefix)
                ? this.LongName
                : $"{config.LongNamePrefix}{this.LongName}");
        
        if (shortInput != null) allowedNames.Add(shortInput);
        if (longInput != null) allowedNames.Add(longInput);

        var argList = config.IsCaseSensitive ? args.ToList() : args.Select(r => r.ToLower()).ToList();
        allowedNames = config.IsCaseSensitive ? allowedNames : allowedNames.Select(r => r.ToLower()).ToList();

        var index = allowedNames.Select(r => argList.IndexOf(r)).Max();

        return index;
    }
}