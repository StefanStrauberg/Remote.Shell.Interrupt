namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor;

public record SNMPGetCommand(string Host,
                             string Community,
                             string OID) : IRequest<Info>;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, Info>
{
    readonly ISNMPCommandExecutor _executor = executor ?? throw new ArgumentNullException(nameof(executor));

    async Task<Info> IRequestHandler<SNMPGetCommand, Info>.Handle(SNMPGetCommand request,
                                                                  CancellationToken cancellationToken)
    {
        return await _executor.GetCommand(request.Host,
                                          request.Community,
                                          request.OID,
                                          cancellationToken);
    }
}
