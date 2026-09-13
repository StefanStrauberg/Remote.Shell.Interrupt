namespace Tests.Validations;

public class UpdateGateCommandValidatorTests
{
    private readonly UpdateGateCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var dto = new UpdateGateDTO
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedGate",
            IPAddress = "192.168.1.1",
            Community = "private",
            TypeOfNetworkDevice = "Cisco"
        };
        var command = new UpdateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var dto = new UpdateGateDTO
        {
            Id = Guid.NewGuid(),
            Name = "",
            IPAddress = "192.168.1.1",
            Community = "public",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new UpdateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-ip")]
    [InlineData("256.1.1.1")]
    [InlineData("192.168.1")]
    public void InvalidIPAddress_FailsValidation(string ipAddress)
    {
        var dto = new UpdateGateDTO
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedGate",
            IPAddress = ipAddress,
            Community = "public",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new UpdateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UpdateGateDTO.IPAddress");
    }

    [Fact]
    public void EmptyId_FailsValidation()
    {
        var dto = new UpdateGateDTO
        {
            Id = Guid.Empty,
            Name = "UpdatedGate",
            IPAddress = "192.168.1.1",
            Community = "public",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new UpdateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UpdateGateDTO.Id");
    }
}
