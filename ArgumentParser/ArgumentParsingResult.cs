namespace ArgumentParser;

public class ArgumentParsingResult<T>
{
    public bool IsSucceed { get; init; }
    public T? Value { get; init; }
    public string? ErrorMessage { get; init; }
    public ArgumentParsingResult(bool isSucceed, T? value, string? errorMessage)
    {
        this.IsSucceed = isSucceed;
        this.Value = value;
        this.ErrorMessage = errorMessage;
    }
}