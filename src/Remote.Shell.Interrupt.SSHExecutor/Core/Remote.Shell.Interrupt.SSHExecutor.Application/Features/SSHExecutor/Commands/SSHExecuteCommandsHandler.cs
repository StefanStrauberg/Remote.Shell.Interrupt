namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.SSHExecutor.Commands;

/// <summary>
/// CQRS command handler implemented business logic of executing ssh commands on remote clien
/// </summary>
/// <typeparam name="SSHExecuteCommandsHandler"></typeparam>
internal class SSHExecuteCommandsHandler(IAppLogger<SSHExecuteCommandsHandler> logger,
                                         ISSHCommandExecutor executor) 
    : ICommandHandler<SSHExecuteCommands, string>
{
    readonly IAppLogger<SSHExecuteCommandsHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ISSHCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<string> IRequestHandler<SSHExecuteCommands, string>.Handle(SSHExecuteCommands request,
                                                                          CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.ExecuteCommands(request.ServerParams,
                                                      request.Commands,
                                                      cancellationToken);
        return response;
    }
}
