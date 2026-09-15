using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.ExecuteWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

namespace Tests.Validations;

public class ExecuteWorkflowCommandValidatorTests
{
    readonly ExecuteWorkflowCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new ExecuteWorkflowCommand(Guid.NewGuid(), "192.168.1.1", "public");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyHost_FailsValidation(string? host)
    {
        var command = new ExecuteWorkflowCommand(Guid.NewGuid(), host!, "public");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Host");
    }

    [Fact]
    public void InvalidIPv4Host_FailsValidation()
    {
        var command = new ExecuteWorkflowCommand(Guid.NewGuid(), "not-an-ip", "public");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("169.254.169.254")]
    [InlineData("224.0.0.1")]
    [InlineData("255.255.255.255")]
    public void DisallowedTargetHost_FailsValidation(string host)
    {
        var command = new ExecuteWorkflowCommand(Guid.NewGuid(), host, "public");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Host");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyCommunity_FailsValidation(string? community)
    {
        var command = new ExecuteWorkflowCommand(Guid.NewGuid(), "192.168.1.1", community!);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Community");
    }
}
