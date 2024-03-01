namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteListCommands;

/// <summary>
/// CQRS command handler implemented business logic of executing many commands on remote server
/// </summary>
/// <typeparam name="ExecuteListCommandsHandler"></typeparam>
internal class ExecuteListCommandsHandler(IAppLogger<ExecuteListCommandsHandler> logger,
                                          ICommandExecutor executor) 
    : ICommandHandler<ExecuteListCommands, Response>
{
    readonly IAppLogger<ExecuteListCommandsHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ICommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<Response> IRequestHandler<ExecuteListCommands, Response>.Handle(ExecuteListCommands request,
                                                                               CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.ExecuteCommands(request.ServerParams,
                                                      request.Commands,
                                                      cancellationToken);
        return response;
    }
}
