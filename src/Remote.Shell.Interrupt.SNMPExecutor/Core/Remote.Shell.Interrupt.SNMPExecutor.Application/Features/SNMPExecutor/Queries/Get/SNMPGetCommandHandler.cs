namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Get;

public record SNMPGetCommand(string Host,
                             string Community,
                             string Oid) : IRequest<JObject>;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, JObject>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<JObject> IRequestHandler<SNMPGetCommand, JObject>.Handle(SNMPGetCommand request,
                                                                        CancellationToken cancellationToken)
    {
        var response = await _executor.GetCommand(request.Host,
                                                  request.Community,
                                                  request.Oid,
                                                  cancellationToken);
        return response;
    }
}
