using ArgumentParser.Attributes;

namespace ArgumentParser.Sample;

public class MyOption
{
    [field: ValuedArgument("p", "para", "The first parameter", null, null, null, false)]
    public string Para1 { get; set; }

    [field: ValuedArgument("o", "open", "The second parameter", null, null, null, false)]
    public string Para2 { get; set; }
}