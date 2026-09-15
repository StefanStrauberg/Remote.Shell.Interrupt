namespace Tests.Application.Helpers;

public class ConverterTests
{
    [Fact]
    public void ArrayToString_MultipleValues_JoinsWithCommaAndSpace()
    {
        Converter.ArrayToString([1, 2, 3]).Should().Be("1, 2, 3");
    }

    [Fact]
    public void ArrayToString_SingleValue_ReturnsValue()
    {
        Converter.ArrayToString([42]).Should().Be("42");
    }

    [Fact]
    public void ArrayToString_EmptySequence_ReturnsEmptyString()
    {
        Converter.ArrayToString(Array.Empty<int>()).Should().Be(string.Empty);
    }

    [Fact]
    public void ArrayToString_NullSequence_ReturnsEmptyString()
    {
        Converter.ArrayToString(null!).Should().Be(string.Empty);
    }
}

public class StringExtensionsTests
{
    [Fact]
    public void ContainsWholeWord_WholeWordPresent_ReturnsTrue()
    {
        "My Test Client".ContainsWholeWord("Test").Should().BeTrue();
    }

    [Fact]
    public void ContainsWholeWord_PartialMatch_ReturnsFalse()
    {
        "MyTestClient catalog".ContainsWholeWord("Test").Should().BeFalse();
        "catalog".ContainsWholeWord("cat").Should().BeFalse();
    }

    [Fact]
    public void ContainsWholeWord_IsCaseInsensitive()
    {
        "My TEST Client".ContainsWholeWord("test").Should().BeTrue();
    }

    [Fact]
    public void ContainsWholeWord_NullOrEmptyInputs_ReturnsFalse()
    {
        ((string)null!).ContainsWholeWord("word").Should().BeFalse();
        "input".ContainsWholeWord("").Should().BeFalse();
        "".ContainsWholeWord("").Should().BeFalse();
    }

    [Fact]
    public void ContainsWholeWord_EscapesRegexSpecialCharactersInPattern()
    {
        // The pattern is regex-escaped: the dot matches only a literal dot, not any character.
        "a.b c".ContainsWholeWord(".b").Should().BeTrue();
        "axb c".ContainsWholeWord(".b").Should().BeFalse();
    }
}
