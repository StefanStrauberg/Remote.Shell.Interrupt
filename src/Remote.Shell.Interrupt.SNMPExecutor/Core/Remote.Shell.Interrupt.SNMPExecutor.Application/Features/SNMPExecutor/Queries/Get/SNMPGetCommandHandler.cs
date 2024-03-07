
namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Get;

/// <summary>
/// CQRS Command - SNMP Get
/// </summary>
public record SNMPGetCommand(SNMPParams SNMPParams,
                             string Oid) 
    : ICommand<string>;

/// <summary>
/// CQRS command handler implemented business logic of executing snmp Get command on remote network device
/// </summary>
internal class SNMPGetCommandHandler(IAppLogger<SNMPGetCommandHandler> logger,
                                     ISNMPCommandExecutor executor) 
    : ICommandHandler<SNMPGetCommand, string>
{
    readonly IAppLogger<SNMPGetCommandHandler> logger = logger
    ?? throw new ArgumentNullException(nameof(logger));
    readonly ISNMPCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));
        
    async Task<string> IRequestHandler<SNMPGetCommand, string>.Handle(SNMPGetCommand request,
                                                                      CancellationToken cancellationToken)
    {
        executor.Notify += logger.LogInformation;
        var response = await executor.GetCommand(request.SNMPParams,
                                                 request.Oid,
                                                 cancellationToken);
        return Helper.Helper.DictionaryToJson(response);
    }
}
