using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace ArgumentParser.Attributes;
/// <summary>
/// Mark current field as an argument which can accept one value.
/// The value should be immediately after the argument's short/long name, and there should be an space between the argument name and value.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ValuedArgumentAttribute : BaseArgumentAttribute
{
    /// <summary>
    /// The regular expression pattern to validate whether the value is valid or not.
    /// </summary>
    private string? Pattern { get; set; }

    /// <summary>
    /// The error message to show if the provided value is invalid according to the Pattern.
    /// </summary>
    private string? ErrorMessage { get; set; }

    /// <summary>
    /// The default value if not provided from the command line.
    /// If the parameter short/long name is provided, the parser will still try to parse the value from command line input.
    /// The parser will try to apply the default value only if the argument's short and long name are both not provided.
    /// </summary>
    private string? DefaultValue { get; set; }

    public ValuedArgumentAttribute(string? shortName, string? longName, string help, string? pattern, string? errorMessage, string? defaultValue, bool isRequired = false)
        :base(shortName, longName, isRequired, help)
    {
        this.Pattern = pattern;
        this.ErrorMessage = errorMessage;
        this.DefaultValue = defaultValue;
    }

    public override IEnumerable<int> SetFieldValue(string[] args, object instance, FieldInfo field, ArgumentParsingConfig config)
    {
        var index = base.GetIndexFromCommandLine(args, config);

        switch (index)
        {
            case -1 when this.IsRequired:
                throw new ArgumentException($"Argument {this.ShortName}/{this.LongName} is required.");
            case -1:
                field.SetValue(instance, string.IsNullOrWhiteSpace(this.DefaultValue) ? default : this.DefaultValue);
                return new int[] { index };
            default:
                if (index + 1 >= args.Length)
                    throw new ArgumentException($"Invalid value provided for {this.ShortName}/{this.LongName}");
                var providedValue = args[index + 1];
                if (string.IsNullOrWhiteSpace(this.Pattern))
                {
                    field.SetValue(instance, providedValue);
                    return new int[] { index, index + 1 };
                }

                if (Regex.IsMatch(providedValue, this.Pattern))
                {
                    field.SetValue(instance, providedValue);
                    return new int[] { index, index + 1 };
                }

                var message = string.IsNullOrWhiteSpace(this.ErrorMessage)
                    ? $"Invalid value provided for {this.ShortName}/{this.LongName}"
                    : this.ErrorMessage;
                throw new ArgumentException(message);
        }
    }
}