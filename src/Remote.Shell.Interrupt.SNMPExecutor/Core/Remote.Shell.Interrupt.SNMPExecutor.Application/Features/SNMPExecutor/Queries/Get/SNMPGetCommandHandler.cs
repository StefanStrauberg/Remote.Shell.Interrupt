namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Get;

public record SNMPGetCommand(string Host,
                             string Community,
                             string OID) : IRequest<Information>;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, Information>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<Information> IRequestHandler<SNMPGetCommand, Information>.Handle(SNMPGetCommand request,
                                                                                CancellationToken cancellationToken)
    {
        return await _executor.GetCommand(request.Host,
                                          request.Community,
                                          request.OID,
                                          cancellationToken);
    }
}
