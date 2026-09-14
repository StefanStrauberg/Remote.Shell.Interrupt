using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

namespace Tests.Validations;

public class LoginCommandValidatorTests
{
    readonly LoginCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new LoginCommand("user@example.com", "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_FailsValidation(string? email)
    {
        var command = new LoginCommand(email!, "password123");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void InvalidPassword_FailsValidation(string? password)
    {
        var command = new LoginCommand("user@example.com", password!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
