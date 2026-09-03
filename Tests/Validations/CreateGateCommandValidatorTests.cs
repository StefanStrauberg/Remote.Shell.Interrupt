namespace Tests.Validations;

public class CreateGateCommandValidatorTests
{
    private readonly CreateGateCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var dto = new CreateGateDTO
        {
            Name = "TestGate",
            IPAddress = "192.168.1.1",
            Community = "public",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new CreateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NullName_FailsValidation()
    {
        var dto = new CreateGateDTO
        {
            Name = null!,
            IPAddress = "192.168.1.1",
            Community = "public",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new CreateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void EmptyCommunity_FailsValidation()
    {
        var dto = new CreateGateDTO
        {
            Name = "Test",
            IPAddress = "192.168.1.1",
            Community = "",
            TypeOfNetworkDevice = "Juniper"
        };
        var command = new CreateGateCommand(dto);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }
}
