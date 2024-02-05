// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;
using ArgumentParser;
using ArgumentParser.Sample;

var parser = new ArgumentParser<MyOption>(new ArgumentParsingConfig("--", "-", true));
var result = parser.TryParse(args);

Console.WriteLine(JsonSerializer.Serialize(result));

if (result.IsSucceed == false)
{
    Console.WriteLine(parser.GetHelpMessage());
}