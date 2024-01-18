using System.Reflection;

namespace ArgumentParser.Attributes;

/// <summary>
/// Mark current field as a flag, which means it doesn't accept any additional value, if this argument is provided in command line.
/// the value for this field will be true, otherwise false.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FlagArgumentAttribute : BaseArgumentAttribute
{
    public FlagArgumentAttribute(string? shortName, string? longName, string help) : base(shortName, longName, false, help)
    { }

    public override IEnumerable<int> SetFieldValue(string[] args, object instance, FieldInfo field, ArgumentParsingConfig config)
    {
        var index = base.GetIndexFromCommandLine(args, config);
        field.SetValue(instance, index >= 0);

        return new int[] { index };
    }
}