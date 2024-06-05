namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Get;

public record SNMPGetCommand(string Host,
                             string Community,
                             string Oid)
    : IRequest<IList<Response>>;

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor)
    : IRequestHandler<SNMPGetCommand, IList<Response>>
{
    readonly ISNMPCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<IList<Response>> IRequestHandler<SNMPGetCommand, IList<Response>>.Handle(SNMPGetCommand request,
                                                                                        CancellationToken cancellationToken)
    {
        var response = await executor.GetCommand(request.Host,
                                                 request.Community,
                                                 request.Oid,
                                                 cancellationToken);
        return response;
    }
}
