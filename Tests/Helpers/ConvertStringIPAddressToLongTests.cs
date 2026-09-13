namespace Tests.Helpers;

public class ConvertStringIPAddressToLongTests
{
    [Fact]
    public void Handle_ValidIPv4_ReturnsCorrectLongValue()
    {
        var result = ConvertStringIPAddressToLong.Handle("192.168.1.1");
        result.Should().Be(3232235777L); // 0xC0A80101
    }

    [Fact]
    public void Handle_LoopbackAddress_ReturnsCorrectLongValue()
    {
        var result = ConvertStringIPAddressToLong.Handle("127.0.0.1");
        result.Should().Be(2130706433L);
    }

    [Fact]
    public void Handle_ZeroAddress_ReturnsZero()
    {
        var result = ConvertStringIPAddressToLong.Handle("0.0.0.0");
        result.Should().Be(0L);
    }

    [Fact]
    public void Handle_BroadcastAddress_ReturnsMaxValue()
    {
        var result = ConvertStringIPAddressToLong.Handle("255.255.255.255");
        result.Should().Be(4294967295L);
    }

    [Fact]
    public void Handle_NullInput_ThrowsArgumentException()
    {
        Action act = () => ConvertStringIPAddressToLong.Handle(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Handle_EmptyInput_ThrowsArgumentException()
    {
        Action act = () => ConvertStringIPAddressToLong.Handle("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Handle_WhitespaceInput_ThrowsArgumentException()
    {
        Action act = () => ConvertStringIPAddressToLong.Handle("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("192.168.1")]
    [InlineData("192.168.1.1.1")]
    public void Handle_InvalidSegmentCount_ThrowsFormatException(string ip)
    {
        Action act = () => ConvertStringIPAddressToLong.Handle(ip);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Handle_InvalidSegment_ThrowsFormatException()
    {
        Action act = () => ConvertStringIPAddressToLong.Handle("192.168.1.abc");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Handle_SegmentOutOfRange_ThrowsFormatException()
    {
        Action act = () => ConvertStringIPAddressToLong.Handle("192.168.1.256");
        act.Should().Throw<FormatException>();
    }
}
