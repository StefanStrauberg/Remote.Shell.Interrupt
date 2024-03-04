namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.SNMPExecutor.Commands;

/// <summary>
/// CQRS command handler implemented business logic of executing snmp commands on remote network device
/// </summary>
/// <typeparam name="SNMPExecuteCommandsHandler"></typeparam>
internal class SNMPExecuteCommandsHandler(IAppLogger<SNMPExecuteCommandsHandler> logger,
                                          ISNMPCommandExecutor executor) 
    : ICommandHandler<SNMPExecuteCommands, string>
{
    readonly IAppLogger<SNMPExecuteCommandsHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ISNMPCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));
        
    async Task<string> IRequestHandler<SNMPExecuteCommands, string>.Handle(SNMPExecuteCommands request,
                                                                           CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.ExecuteCommands(request.SNMPParams,
                                                      request.Commands,
                                                      cancellationToken);
        return response;
    }
}
