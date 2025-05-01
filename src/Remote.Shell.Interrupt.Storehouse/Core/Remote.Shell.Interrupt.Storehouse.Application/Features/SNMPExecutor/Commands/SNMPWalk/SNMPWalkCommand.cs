namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPWalk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<IEnumerable<SNMPResponse>>;
                                    
internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, IEnumerable<SNMPResponse>>
{
    readonly ISNMPCommandExecutor _executor = executor ?? throw new ArgumentNullException(nameof(executor));

    async Task<IEnumerable<SNMPResponse>> IRequestHandler<SNMPWalkCommand, IEnumerable<SNMPResponse>>.Handle(SNMPWalkCommand request,
                                                                                                             CancellationToken cancellationToken)
    {
        return await _executor.WalkCommand(request.Host,
                                           request.Community,
                                           request.OID,
                                           cancellationToken);
    }
}
