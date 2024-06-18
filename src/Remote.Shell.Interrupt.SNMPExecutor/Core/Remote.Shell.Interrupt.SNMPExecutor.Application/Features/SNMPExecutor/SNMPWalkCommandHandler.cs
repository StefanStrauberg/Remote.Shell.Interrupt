namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<IEnumerable<Info>>;

internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, IEnumerable<Info>>
{
    readonly ISNMPCommandExecutor _executor = executor ?? throw new ArgumentNullException(nameof(executor));

    async Task<IEnumerable<Info>> IRequestHandler<SNMPWalkCommand, IEnumerable<Info>>.Handle(SNMPWalkCommand request,
                                                                                             CancellationToken cancellationToken)
    {
        return await _executor.WalkCommand(request.Host,
                                           request.Community,
                                           request.OID,
                                           cancellationToken);
    }
}
