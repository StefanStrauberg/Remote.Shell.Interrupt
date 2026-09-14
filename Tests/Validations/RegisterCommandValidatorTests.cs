using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Register;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

namespace Tests.Validations;

public class RegisterCommandValidatorTests
{
    readonly RegisterCommandValidator _validator = new();

    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    public void ValidCommand_PassesValidation(string role)
    {
        var command = new RegisterCommand("user@example.com", "a-strong-password", role);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_FailsValidation(string? email)
    {
        var command = new RegisterCommand(email!, "a-strong-password", "User");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("short1")]
    public void InvalidPassword_FailsValidation(string? password)
    {
        var command = new RegisterCommand("user@example.com", password!, "User");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("SuperAdmin")]
    public void InvalidRole_FailsValidation(string? role)
    {
        var command = new RegisterCommand("user@example.com", "a-strong-password", role!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }
}
