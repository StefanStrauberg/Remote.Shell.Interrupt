namespace Tests.Helpers;

public class ConvertLongIPAddressToStringTests
{
    [Fact]
    public void Handle_ValidLongIP_ReturnsCorrectString()
    {
        var result = ConvertLongIPAddressToString.Handle(3232235777L);
        result.Should().Be("192.168.1.1");
    }

    [Fact]
    public void Handle_LoopbackLongIP_ReturnsCorrectString()
    {
        var result = ConvertLongIPAddressToString.Handle(2130706433L);
        result.Should().Be("127.0.0.1");
    }

    [Fact]
    public void Handle_ZeroLongIP_ReturnsZeroString()
    {
        var result = ConvertLongIPAddressToString.Handle(0L);
        result.Should().Be("0.0.0.0");
    }

    [Fact]
    public void Handle_BroadcastLongIP_ReturnsBroadcastString()
    {
        var result = ConvertLongIPAddressToString.Handle(4294967295L);
        result.Should().Be("255.255.255.255");
    }

    [Fact]
    public void Handle_NegativeValue_ThrowsArgumentException()
    {
        Action act = () => ConvertLongIPAddressToString.Handle(-1L);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Handle_ValueExceedingUInt32Max_ThrowsArgumentException()
    {
        Action act = () => ConvertLongIPAddressToString.Handle((long)uint.MaxValue + 1);
        act.Should().Throw<ArgumentException>();
    }
}
