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

public class FormatEgressPortsJuniperTests
{
    [Fact]
    public void HandleJuniperData_CommaSeparatedValues_ParsesAllPorts()
    {
        FormatEgressPorts.HandleJuniperData("1, 2, 3").Should().Equal(1, 2, 3);
    }

    [Fact]
    public void HandleJuniperData_SpaceSeparatedWithoutCommas_ParsesAllPorts()
    {
        FormatEgressPorts.HandleJuniperData("5 10 15").Should().Equal(5, 10, 15);
    }

    [Fact]
    public void HandleJuniperData_SingleValue_ReturnsSinglePort()
    {
        FormatEgressPorts.HandleJuniperData("7").Should().Equal(7);
    }

    [Fact]
    public void HandleJuniperData_NullOrWhitespace_ReturnsEmptyArray()
    {
        FormatEgressPorts.HandleJuniperData(null!).Should().BeEmpty();
        FormatEgressPorts.HandleJuniperData("   ").Should().BeEmpty();
    }

    [Fact]
    public void HandleJuniperData_LeadingAndTrailingSeparators_Ignored()
    {
        FormatEgressPorts.HandleJuniperData(", 4, 8, ").Should().Equal(4, 8);
    }

    [Fact]
    public void HandleJuniperData_NonNumericInput_ThrowsFormatException()
    {
        var act = () => FormatEgressPorts.HandleJuniperData("abc");
        act.Should().Throw<FormatException>();
    }
}

public class OIDGetNumbersTests
{
    [Fact]
    public void HandleLast_ReturnsLastSegment()
    {
        OIDGetNumbers.HandleLast("1.3.6.1.2.1.17.7.1.4.3.1.1.100").Should().Be(100);
    }

    [Fact]
    public void HandleLast_SingleSegment_ReturnsValue()
    {
        OIDGetNumbers.HandleLast("42").Should().Be(42);
    }

    [Fact]
    public void HandleLastButOne_ReturnsPenultimateSegment()
    {
        OIDGetNumbers.HandleLastButOne("1.3.6.1.2.1.31.1.2.1.3.3.1").Should().Be(3);
    }

    [Fact]
    public void HandleLast_NonNumericSegment_ThrowsFormatException()
    {
        var act = () => OIDGetNumbers.HandleLast("1.3.6.abc");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void HandleLastButOne_SingleSegment_ThrowsIndexOutOfRangeException()
    {
        var act = () => OIDGetNumbers.HandleLastButOne("42");
        act.Should().Throw<IndexOutOfRangeException>();
    }
}

public class RemoveTrailingPlusDigitTests
{
    [Fact]
    public void Handle_TrailingPlusDigit_RemovesSuffix()
    {
        RemoveTrailingPlusDigit.Handle("text+5").Should().Be("text");
    }

    [Fact]
    public void Handle_MultipleSuffixes_RemovesOnlyLast()
    {
        RemoveTrailingPlusDigit.Handle("a+1+2").Should().Be("a+1");
    }

    [Fact]
    public void Handle_TrailingPlusNonDigit_PreservesInput()
    {
        RemoveTrailingPlusDigit.Handle("text+X").Should().Be("text+X");
    }

    [Fact]
    public void Handle_PlusAtEnd_PreservesInput()
    {
        RemoveTrailingPlusDigit.Handle("text+").Should().Be("text+");
    }

    [Fact]
    public void Handle_NoPlus_PreservesInput()
    {
        RemoveTrailingPlusDigit.Handle("text5").Should().Be("text5");
    }

    [Fact]
    public void Handle_NullOrEmpty_ReturnsAsIs()
    {
        RemoveTrailingPlusDigit.Handle((string)null!).Should().BeNull();
        RemoveTrailingPlusDigit.Handle("").Should().BeEmpty();
    }

    [Fact]
    public void Handle_PlusDigitFollowedByDigitAnywhere_StripsSuffix()
    {
        // The implementation strips at the last '+' whenever the next character is a digit,
        // even when more characters follow it.
        RemoveTrailingPlusDigit.Handle("text+5x").Should().Be("text");
    }
}
