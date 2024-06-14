namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Walk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<JsonArray>;

internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, JsonArray>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<JsonArray> IRequestHandler<SNMPWalkCommand, JsonArray>.Handle(SNMPWalkCommand request,
                                                                             CancellationToken cancellationToken)
    {
        return await _executor.WalkCommand(request.Host,
                                           request.Community,
                                           request.OID,
                                           cancellationToken);
    }
}
