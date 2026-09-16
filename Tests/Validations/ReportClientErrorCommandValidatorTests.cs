using Remote.Shell.Interrupt.Storehouse.Application.Features.Diagnostics.Commands.ReportClientError;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Diagnostics;

namespace Tests.Validations;

public class ReportClientErrorCommandValidatorTests
{
    readonly ReportClientErrorCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new ReportClientErrorCommand(
            "TypeError: x is not a function", "at foo (app.js:1:1)",
            "https://app.example/workflows/1", "Mozilla/5.0", "{\"boundary\":\"application\"}");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void OnlyRequiredMessage_PassesValidation()
    {
        var command = new ReportClientErrorCommand("Something broke", null, null, null, null);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyMessage_FailsValidation(string? message)
    {
        var command = new ReportClientErrorCommand(message!, null, null, null, null);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Message");
    }

    [Fact]
    public void OverlongMessage_FailsValidation()
    {
        var command = new ReportClientErrorCommand(new string('x', 2_001), null, null, null, null);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Message");
    }

    [Fact]
    public void OverlongStack_FailsValidation()
    {
        var command = new ReportClientErrorCommand("boom", new string('x', 8_001), null, null, null);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Stack");
    }
}
