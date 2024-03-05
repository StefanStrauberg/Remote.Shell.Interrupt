namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Commands;

/// <summary>
/// CQRS command handler implemented business logic of executing snmp commands on remote network device
/// </summary>
/// <typeparam name="SNMPExecuteCommandsHandler"></typeparam>
internal class SNMPWalkCommandHandler(IAppLogger<SNMPWalkCommandHandler> logger,
                                      ISNMPCommandExecutor executor) 
    : ICommandHandler<SNMPWalkCommand, string>
{
    readonly IAppLogger<SNMPWalkCommandHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ISNMPCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));
        
    async Task<string> IRequestHandler<SNMPWalkCommand, string>.Handle(SNMPWalkCommand request,
                                                                       CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.WalkCommand(request.SNMPParams,
                                                  request.Command,
                                                  cancellationToken);
        return response;
    }
}
