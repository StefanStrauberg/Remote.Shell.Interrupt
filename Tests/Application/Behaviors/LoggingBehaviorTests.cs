using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Behaviors;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;

namespace Tests.Application.Behaviors;

public record TestLogCommand(string Payload) : IRequest<string>;

public class LoggingBehaviorTests
{
    readonly IAppLogger<LoggingBehavior<TestLogCommand, string>> _logger =
        Substitute.For<IAppLogger<LoggingBehavior<TestLogCommand, string>>>();
    readonly LoggingBehavior<TestLogCommand, string> _behavior;
    readonly TestLogCommand _command = new("payload");

    public LoggingBehaviorTests()
    {
        _behavior = new LoggingBehavior<TestLogCommand, string>(_logger);
    }

    [Fact]
    public async Task Handle_NextSucceeds_ReturnsResponse()
    {
        var result = await _behavior.Handle(_command, () => Task.FromResult("ok"), CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_NextSucceeds_LogsStartAndEnd()
    {
        await _behavior.Handle(_command, () => Task.FromResult("ok"), CancellationToken.None);

        _logger.Received().LogInformation(Arg.Is<string>(m => m.Contains("[START]")), Arg.Any<object[]>());
        _logger.Received().LogInformation(Arg.Is<string>(m => m.Contains("[END]")), Arg.Any<object[]>());
        _logger.DidNotReceive().LogError(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Fact]
    public async Task Handle_NextThrows_LogsErrorAndRethrows()
    {
        var act = async () => await _behavior.Handle(_command,
                                                     () => throw new InvalidOperationException("boom"),
                                                     CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
        _logger.Received().LogError(Arg.Is<string>(m => m.Contains("[ERROR]")), Arg.Any<object[]>());
    }
}

public record TestLogUnitCommand : ICommand<Unit>;

public class LoggingBehaviorUnitTests
{
    [Fact]
    public async Task Handle_UnitResponse_PassesThrough()
    {
        var logger = Substitute.For<IAppLogger<LoggingBehavior<TestLogUnitCommand, Unit>>>();
        var behavior = new LoggingBehavior<TestLogUnitCommand, Unit>(logger);

        var result = await behavior.Handle(new TestLogUnitCommand(),
                                           () => Task.FromResult(Unit.Value),
                                           CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}
