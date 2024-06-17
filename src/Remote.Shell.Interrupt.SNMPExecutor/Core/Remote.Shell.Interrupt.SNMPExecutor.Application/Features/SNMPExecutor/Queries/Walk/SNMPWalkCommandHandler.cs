namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Walk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<IEnumerable<Information>>;

internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, IEnumerable<Information>>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<IEnumerable<Information>> IRequestHandler<SNMPWalkCommand, IEnumerable<Information>>.Handle(SNMPWalkCommand request,
                                                                                                           CancellationToken cancellationToken)
    {
        return await _executor.WalkCommand(request.Host,
                                           request.Community,
                                           request.OID,
                                           cancellationToken);
    }
}
