namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Get;

public record SNMPGetCommand(string Host,
                             string Community,
                             string OID) : IRequest<JsonObject>;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, JsonObject>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<JsonObject> IRequestHandler<SNMPGetCommand, JsonObject>.Handle(SNMPGetCommand request,
                                                                              CancellationToken cancellationToken)
    {
        return await _executor.GetCommand(request.Host,
                                          request.Community,
                                          request.OID,
                                          cancellationToken);
    }
}
