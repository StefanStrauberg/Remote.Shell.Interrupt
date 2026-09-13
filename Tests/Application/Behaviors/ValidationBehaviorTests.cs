using FluentValidation;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Behaviors;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using ValidationException = Remote.Shell.Interrupt.Storehouse.Application.Exceptions.ValidationException;

namespace Tests.Application.Behaviors;

public record TestValidatedCommand(string Value) : ICommand<string>;

public class ValidationBehaviorTests
{
    static IValidator<TestValidatedCommand> PassingValidator()
    {
        var validator = Substitute.For<IValidator<TestValidatedCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestValidatedCommand>>(), Arg.Any<CancellationToken>())
                 .Returns(new FluentValidation.Results.ValidationResult());
        return validator;
    }

    static IValidator<TestValidatedCommand> FailingValidator(string property, string message)
    {
        var validator = Substitute.For<IValidator<TestValidatedCommand>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestValidatedCommand>>(), Arg.Any<CancellationToken>())
                 .Returns(new FluentValidation.Results.ValidationResult(
                              [new FluentValidation.Results.ValidationFailure(property, message)]));
        return validator;
    }

    [Fact]
    public async Task Handle_NoValidators_InvokesNext()
    {
        var behavior = new ValidationBehavior<TestValidatedCommand, string>([]);
        var nextCalled = false;

        var result = await behavior.Handle(new TestValidatedCommand("v"),
                                           () => { nextCalled = true; return Task.FromResult("ok"); },
                                           CancellationToken.None);

        result.Should().Be("ok");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidatorPasses_InvokesNext()
    {
        var behavior = new ValidationBehavior<TestValidatedCommand, string>([PassingValidator()]);

        var result = await behavior.Handle(new TestValidatedCommand("v"),
                                           () => Task.FromResult("ok"),
                                           CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_ValidatorFails_ThrowsValidationExceptionWithErrors()
    {
        var behavior = new ValidationBehavior<TestValidatedCommand, string>([FailingValidator("Value", "is bad")]);

        var act = async () => await behavior.Handle(new TestValidatedCommand("v"),
                                                    () => Task.FromResult("ok"),
                                                    CancellationToken.None);

        var exception = (await act.Should().ThrowAsync<ValidationException>()).Which;
        exception.ErrorsDictionary.Should().ContainKey("Value");
        exception.ErrorsDictionary["Value"].Should().Contain("is bad");
    }

    [Fact]
    public async Task Handle_MultipleValidatorsFailing_GroupsErrorsByProperty()
    {
        var behavior = new ValidationBehavior<TestValidatedCommand, string>(
            [FailingValidator("Value", "first"), FailingValidator("Value", "second"),
             FailingValidator("Other", "other")]);

        var act = async () => await behavior.Handle(new TestValidatedCommand("v"),
                                                    () => Task.FromResult("ok"),
                                                    CancellationToken.None);

        var exception = (await act.Should().ThrowAsync<ValidationException>()).Which;
        exception.ErrorsDictionary.Should().HaveCount(2);
        exception.ErrorsDictionary["Value"].Should().BeEquivalentTo(["first", "second"]);
        exception.ErrorsDictionary["Other"].Should().BeEquivalentTo(["other"]);
    }

    [Fact]
    public async Task Handle_ValidationFails_NextIsNeverInvoked()
    {
        var behavior = new ValidationBehavior<TestValidatedCommand, string>([FailingValidator("Value", "bad")]);
        var nextCalled = false;

        var act = async () => await behavior.Handle(new TestValidatedCommand("v"),
                                                    () => { nextCalled = true; return Task.FromResult("ok"); },
                                                    CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        nextCalled.Should().BeFalse();
    }
}
