namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Walk;

/// <summary>
/// CQRS Command - SNMP Walk
/// </summary>
public record class SNMPWalkCommand(SNMPParams SNMPParams,
                                    string Oid) 
    : ICommand<string>;

/// <summary>
/// CQRS command handler implemented business logic of executing snmp Walk command on remote network device
/// </summary>
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
                                                  request.Oid,
                                                  cancellationToken);
        return Helper.Helper.DictionaryToJson(response);
    }
}
