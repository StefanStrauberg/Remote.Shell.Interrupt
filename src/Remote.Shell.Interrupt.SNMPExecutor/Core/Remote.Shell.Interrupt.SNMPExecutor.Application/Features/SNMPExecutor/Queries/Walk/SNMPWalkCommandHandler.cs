namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Walk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string Oid)
    : IRequest<IList<Response>>;

internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor)
    : IRequestHandler<SNMPWalkCommand, IList<Response>>
{
    readonly ISNMPCommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<IList<Response>> IRequestHandler<SNMPWalkCommand, IList<Response>>.Handle(SNMPWalkCommand request,
                                                                                         CancellationToken cancellationToken)
    {
        var response = await executor.WalkCommand(request.Host,
                                                  request.Community,
                                                  request.Oid,
                                                  cancellationToken);
        return response;
    }
}
