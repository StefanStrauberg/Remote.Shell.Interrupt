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
}
