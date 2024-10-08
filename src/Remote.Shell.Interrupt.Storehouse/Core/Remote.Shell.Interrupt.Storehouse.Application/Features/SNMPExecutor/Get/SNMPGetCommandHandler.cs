namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Get;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, SNMPResponse>
{
    readonly ISNMPCommandExecutor _executor = executor ?? throw new ArgumentNullException(nameof(executor));

    async Task<SNMPResponse> IRequestHandler<SNMPGetCommand, SNMPResponse>.Handle(SNMPGetCommand request,
                                                                                  CancellationToken cancellationToken)
    {
        return await _executor.GetCommand(request.Host,
                                          request.Community,
                                          request.OID,
                                          cancellationToken);
    }
}
