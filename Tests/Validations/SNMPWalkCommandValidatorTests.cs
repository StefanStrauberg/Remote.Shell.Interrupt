namespace Tests.Validations;

public class SNMPWalkCommandValidatorTests
{
    private readonly SNMPWalkCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new SNMPWalkCommand("192.168.1.1", "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyHost_FailsValidation(string? host)
    {
        var command = new SNMPWalkCommand(host!, "public", "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyCommunity_FailsValidation(string? community)
    {
        var command = new SNMPWalkCommand("192.168.1.1", community!, "1.3.6.1.2.1.1.1.0");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyOID_FailsValidation(string? oid)
    {
        var command = new SNMPWalkCommand("192.168.1.1", "public", oid!);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void InvalidOIDFormat_FailsValidation()
    {
        var command = new SNMPWalkCommand("192.168.1.1", "public", "bad-oid");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }
}
