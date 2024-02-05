using ArgumentParser;
using ArgumentParser.Attributes;

namespace ArgumentParser.Test;

[TestFixture]
[TestOf(typeof(ArgumentParser<>))]
public class ArgumentParserTest
{
    [Test]
    public void ConstructorTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var parser = new ArgumentParser<object>(null);
        });
    }

    private class NonParsableOption
    {
        public string Para1 { get; set; }
        public string Para2 { get; set; }
    }
    [Test]
    public void NonParsableTest()
    {
        var config = new ArgumentParsingConfig("", "", true);
        var parser = new ArgumentParser<NonParsableOption>(config);
        var result = parser.TryParse(new string[] { "1" });

        var expected = new ArgumentParsingResult<NonParsableOption>(false, null, $"No parsable fields defined on: {typeof(NonParsableOption)}");
        Assert.Multiple(() =>
        {
            Assert.That(result.ErrorMessage, Is.EqualTo(expected.ErrorMessage));
            Assert.That(result.IsSucceed, Is.EqualTo(expected.IsSucceed));
            Assert.That(result.Value, Is.EqualTo(expected.Value));
        });
    }

    private class ParsableOption
    {
        [ValuedArgument("p", "param", "The parameter", null, null, null, true)]
        private string _para1;
        public string Para1
        {
            get => _para1;
            set => _para1 = value;
        }
        
    }
    [Test]
    public void ParsableTest()
    {
        var config = new ArgumentParsingConfig("", "", true);
        var parser = new ArgumentParser<ParsableOption>(config);
        var result = parser.TryParse(new string[] { "p", "input-value" });

        var expected =
            new ArgumentParsingResult<ParsableOption>(true, new ParsableOption() { Para1 = "input-value" }, null);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.ErrorMessage, Is.EqualTo(expected.ErrorMessage));
            Assert.That(result.IsSucceed, Is.EqualTo(expected.IsSucceed));
            Assert.That(result.Value, Is.EqualTo(expected.Value));
        });
    }
}