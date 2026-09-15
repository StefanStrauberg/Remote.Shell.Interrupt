using Remote.Shell.Interrupt.Storehouse.Application.Validations.SNMP;

namespace Tests.Validations;

public class SnmpTargetGuardTests
{
    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("172.16.5.9")]
    [InlineData("192.168.1.1")]
    [InlineData("192.168.101.8")]
    [InlineData("8.8.8.8")]
    [InlineData("1.1.1.1")]
    [InlineData("223.255.255.255")] // last address before multicast range
    public void RealDeviceAddresses_AreAllowed(string host)
        => SnmpTargetGuard.IsAllowedTarget(host).Should().BeTrue();

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("127.255.255.255")]
    [InlineData("169.254.0.1")]
    [InlineData("169.254.169.254")]
    [InlineData("169.254.255.255")]
    [InlineData("0.0.0.0")]
    [InlineData("0.255.255.255")]
    [InlineData("224.0.0.1")]
    [InlineData("239.255.255.255")]
    [InlineData("240.0.0.1")]
    [InlineData("255.255.255.255")]
    public void NonDeviceAddresses_AreRejected(string host)
        => SnmpTargetGuard.IsAllowedTarget(host).Should().BeFalse();

    [Theory]
    [InlineData("not-an-ip")]
    [InlineData("")]
    [InlineData("::1")] // IPv6 - the Host regex on every validator already excludes this, checked here for defense in depth
    public void NonIPv4Input_IsRejected(string host)
        => SnmpTargetGuard.IsAllowedTarget(host).Should().BeFalse();
}
