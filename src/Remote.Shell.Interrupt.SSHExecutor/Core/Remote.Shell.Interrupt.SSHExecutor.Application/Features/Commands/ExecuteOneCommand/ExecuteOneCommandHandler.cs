namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteOneCommand;

/// <summary>
/// CQRS command handler implemented business logic of executing command on remote server
/// </summary>
/// <typeparam name="ExecuteOneCommandHandler"></typeparam>
internal class ExecuteOneCommandHandler(IAppLogger<ExecuteOneCommandHandler> logger,
                                        ICommandExecutor executor) 
    : ICommandHandler<ExecuteOneCommand, Response>
{
    readonly IAppLogger<ExecuteOneCommandHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ICommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<Response> IRequestHandler<ExecuteOneCommand, Response>.Handle(ExecuteOneCommand request,
                                                                             CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.ExecuteCommand(request.ServerParams,
                                                     request.Command,
                                                     cancellationToken);
        return response;
    }
}