namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteOneCommand;

/// <summary>
/// CQRS command handler to execut command on remote server
/// </summary>
/// <typeparam name="ExecuteOneCommandHandler"></typeparam>
internal class ExecuteOneCommandHandler(IAppLogger<ExecuteOneCommandHandler> logger,
                                        ICommandExecutor executor) : ICommandHandler<ExecuteOneCommand, Response>
{
    readonly IAppLogger<ExecuteOneCommandHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ICommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    Task<Response> IRequestHandler<ExecuteOneCommand, Response>.Handle(ExecuteOneCommand request,
                                                                       CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}