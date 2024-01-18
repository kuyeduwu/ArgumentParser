namespace ArgumentParser;

public class ArgumentParsingConfig
{
    public string? ShortNamePrefix { get; init; }
    public string? LongNamePrefix { get; init; }
    public bool IsCaseSensitive { get; init; }

    public ArgumentParsingConfig(string? longNamePrefix, string? shortNamePrefix, bool isCaseSensitive)
    {
        ShortNamePrefix = shortNamePrefix;
        LongNamePrefix = longNamePrefix;
        IsCaseSensitive = isCaseSensitive;
    }
}