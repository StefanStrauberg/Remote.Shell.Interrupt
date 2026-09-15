namespace Tests.Validations;

public class SNMPGetCommandValidatorTests
{
    private readonly SNMPGetCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new SNMPGetCommand("192.168.1.1", "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyHost_FailsValidation(string? host)
    {
        var command = new SNMPGetCommand(host!, "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Host");
    }

    [Fact]
    public void InvalidIPv4Host_FailsValidation()
    {
        var command = new SNMPGetCommand("not-an-ip", "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyCommunity_FailsValidation(string? community)
    {
        var command = new SNMPGetCommand("192.168.1.1", community!, "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Community");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyOID_FailsValidation(string? oid)
    {
        var command = new SNMPGetCommand("192.168.1.1", "public", oid!);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OID");
    }

    [Fact]
    public void InvalidOIDFormat_FailsValidation()
    {
        var command = new SNMPGetCommand("192.168.1.1", "public", "not-an-oid");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("127.0.0.1")]         // loopback: the API host itself
    [InlineData("169.254.169.254")]   // link-local: cloud metadata endpoint
    [InlineData("224.0.0.1")]         // multicast
    [InlineData("255.255.255.255")]   // reserved / broadcast
    [InlineData("0.0.0.0")]           // "this network"
    public void DisallowedTargetHost_FailsValidation(string host)
    {
        var command = new SNMPGetCommand(host, "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Host");
    }

    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("172.16.0.1")]
    [InlineData("192.168.101.8")] // SNMP simulator's subnet, see docker-compose.yml
    [InlineData("8.8.8.8")]       // a real device may sit on a public IP too
    public void RealDeviceRangeHost_PassesValidation(string host)
    {
        var command = new SNMPGetCommand(host, "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
