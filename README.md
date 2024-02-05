# Argument Parser

A .Net class library to make the command line argument definning and parsing easy.

Console or command line application are usually very efficient tool either for internal or external usage, and a good console application usually can receive numbers of arguments, which is used to control the behavior of the app.

One or two arguments is not a big deal, but as the number of arguments keeps growing, it's time to find a more efficient way to define and parse the arguments.

# Quick Start
1. Create the argument / option class, and apply the attributes.
```cs
using ArgumentParser.Attributes;

public class MyOption {
    [field: ValuedArgument(shortName:"p", longName:"para1", help:"The first parameter", pattern:null, errorMessage:null, defaultValue:null, isRequired:false)]
    public string Para1 {get; set; }

    [field: ValuedArgument(shortName:"p", longName:"para1", help:"The first parameter", pattern:null, errorMessage:null, defaultValue:null, isRequired:false)]
    public string Para2 {get; set; }
}
```

2. Set the argument parsing config and initialize the parser class and do paring.
```cs
using ArgumentParser;

internal static class Main(string[] args) {
    var config = new ArgumentParsingConfig(longNamePrefix:"--", shortNamePrefix:"-", isCaseSensitive:true);
    var parser = new ArgumentParser<MyOption>(config);
    var result = parser.TryParse(args);

    if (!result.IsSucceed) {
        Console.WriteLine(parser.GetHelpMessage());
        return;
    }

    //you logic here
}
```