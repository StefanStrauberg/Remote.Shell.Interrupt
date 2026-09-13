namespace Tests.Helpers;

public class FormatMACAddressTests
{
    [Fact]
    public void Handle_FormatsMACAddressWithSpaces_ReturnsFormattedWithColons()
    {
        var result = FormatMACAddress.Handle("00 1A 2B 3C 4D 5E");
        result.Should().Be("00:1A:2B:3C:4D:5E");
    }

    [Fact]
    public void Handle_FormatsMACAddressWithoutSpaces_ReturnsFormattedWithColons()
    {
        var result = FormatMACAddress.Handle("001A2B3C4D5E");
        result.Should().Be("00:1A:2B:3C:4D:5E");
    }

    [Fact]
    public void Handle_FormatsLowercaseMACAddress_ReturnsUppercaseWithColons()
    {
        var result = FormatMACAddress.Handle("001a2b3c4d5e");
        result.Should().Be("00:1a:2b:3c:4d:5e");
    }

    [Fact]
    public void Handle_NullInput_ThrowsNullReferenceException()
    {
        Action act = () => FormatMACAddress.Handle(null!);
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Handle_EmptyInput_ReturnsEmptyString()
    {
        var result = FormatMACAddress.Handle("");
        result.Should().Be("");
    }

    [Fact]
    public void HandleMACTable_ValidOID_ReturnsMACAddress()
    {
        var oid = "1.3.6.1.2.1.17.4.3.1.2.0.26.43.60.77.94";
        var result = FormatMACAddress.HandleMACTable(oid);
        result.Should().Be("00:1A:2B:3C:4D:5E");
    }

    [Fact]
    public void HandleMACTable_WrongPrefix_ThrowsArgumentException()
    {
        var oid = "1.3.6.1.2.1.17.4.3.1.3.0.26.43.60.77.94";
        Action act = () => FormatMACAddress.HandleMACTable(oid);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void HandleMACTable_TooShortOID_ThrowsArgumentException()
    {
        var oid = "1.3.6.1.2.1";
        Action act = () => FormatMACAddress.HandleMACTable(oid);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void HandleMACTable_InvalidByteValue_ThrowsArgumentException()
    {
        var oid = "1.3.6.1.2.1.17.4.3.1.2.0.26.43.60.77.abc";
        Action act = () => FormatMACAddress.HandleMACTable(oid);
        act.Should().Throw<ArgumentException>();
    }
}
