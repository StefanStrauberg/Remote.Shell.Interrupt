using Remote.Shell.Interrupt.Storehouse.Application.Features.Diagnostics.Commands.ReportClientError;

namespace Tests.Application.Features.Diagnostics;

public class ReportClientErrorCommandHandlerTests
{
    // ReportClientErrorCommandHandler is internal to the Application assembly - see the
    // identical workaround (and its rationale) in NetworkDeviceQueryHandlerTests.cs.
    class RecordingAppLogger<T> : IAppLogger<T>
    {
        public List<(string Message, object[] Args)> ErrorCalls { get; } = [];
        public void LogInformation(string message, params object[] args) { }
        public void LogWarning(string message, params object[] args) { }
        public void LogError(string message, params object[] args) => ErrorCalls.Add((message, args));
        public void LogError(Exception exception, string message, params object[] args) { }
    }

    readonly RecordingAppLogger<ReportClientErrorCommandHandler> _logger = new();
    readonly ReportClientErrorCommandHandler _handler;

    public ReportClientErrorCommandHandlerTests()
    {
        _handler = new ReportClientErrorCommandHandler(_logger);
    }

    [Fact]
    public async Task Handle_LogsErrorWithMessageAndContext()
    {
        var command = new ReportClientErrorCommand(
            "TypeError: x is not a function",
            "at foo (app.js:1:1)",
            "https://app.example/workflows/1",
            "Mozilla/5.0",
            "{\"boundary\":\"application\"}");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
        _logger.ErrorCalls.Should().ContainSingle();
        var (_, args) = _logger.ErrorCalls[0];
        args.Should().Contain(command.Message);
        args.Should().Contain(command.Url!);
        args.Should().Contain(command.UserAgent!);
        args.Should().Contain(command.Context!);
        args.Should().Contain(command.Stack!);
    }

    [Fact]
    public async Task Handle_MissingOptionalFields_LogsPlaceholdersInsteadOfNull()
    {
        var command = new ReportClientErrorCommand("Something broke", null, null, null, null);

        await _handler.Handle(command, CancellationToken.None);

        var (_, args) = _logger.ErrorCalls[0];
        args.Should().NotContain(a => a == null);
    }
}
