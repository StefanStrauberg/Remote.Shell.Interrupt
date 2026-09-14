namespace Tests.Domain;

public class TerminatedNetworkEntityTests
{
    [Fact]
    public void SetAddressAndMask_ValidIPv4_SetsCorrectValues()
    {
        var entity = new TerminatedNetworkEntity();
        entity.SetAddressAndMask("192.168.1.1", "255.255.255.0");

        entity.NetworkAddress.Should().Be(3232235777L);
        entity.Netmask.Should().Be(4294967040L);
    }

    [Fact]
    public void SetAddressAndMask_InvalidIPAddress_ThrowsArgumentException()
    {
        var entity = new TerminatedNetworkEntity();
        Action act = () => entity.SetAddressAndMask("not-an-ip", "255.255.255.0");
        act.Should().Throw<ArgumentException>().WithParameterName("ipAddress");
    }

    [Fact]
    public void SetAddressAndMask_InvalidNetmask_ThrowsArgumentException()
    {
        var entity = new TerminatedNetworkEntity();
        Action act = () => entity.SetAddressAndMask("192.168.1.1", "not-a-mask");
        act.Should().Throw<ArgumentException>().WithParameterName("netmask");
    }

    [Fact]
    public void SetAddressAndMask_IPv6Address_ThrowsArgumentException()
    {
        var entity = new TerminatedNetworkEntity();
        Action act = () => entity.SetAddressAndMask("::1", "255.255.255.0");
        act.Should().Throw<ArgumentException>().WithMessage("*IPv4*");
    }

    [Fact]
    public void SetAddressAndMask_IPv6Netmask_ThrowsArgumentException()
    {
        var entity = new TerminatedNetworkEntity();
        Action act = () => entity.SetAddressAndMask("192.168.1.1", "::1");
        act.Should().Throw<ArgumentException>().WithMessage("*IPv4*").WithParameterName("netmask");
    }

    [Theory]
    [InlineData("255.255.0.255")]
    [InlineData("0.255.255.0")]
    [InlineData("255.0.255.255")]
    [InlineData("192.168.1.1")]
    public void SetAddressAndMask_NonContiguousMask_ThrowsArgumentException(string netmask)
    {
        var entity = new TerminatedNetworkEntity();
        Action act = () => entity.SetAddressAndMask("192.168.1.1", netmask);
        act.Should().Throw<ArgumentException>().WithParameterName("netmask");
    }

    [Theory]
    [InlineData("0.0.0.0", 0L)]
    [InlineData("255.255.255.255", 4294967295L)]
    [InlineData("255.255.255.252", 4294967292L)]
    public void SetAddressAndMask_ContiguousMask_SetsCorrectValue(string netmask, long expected)
    {
        var entity = new TerminatedNetworkEntity();
        entity.SetAddressAndMask("192.168.1.1", netmask);

        entity.Netmask.Should().Be(expected);
    }
}
