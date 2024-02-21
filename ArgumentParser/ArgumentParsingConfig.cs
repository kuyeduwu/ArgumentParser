namespace ArgumentParser;

public class ArgumentParsingConfig
{
    public string? ShortNamePrefix { get; }
    public string? LongNamePrefix { get; }
    public bool IsCaseSensitive { get; }
    public ArgumentParsingConfig(string? longNamePrefix, string? shortNamePrefix, bool isCaseSensitive)
    {
        ShortNamePrefix = shortNamePrefix;
        LongNamePrefix = longNamePrefix;
        IsCaseSensitive = isCaseSensitive;
    }
}