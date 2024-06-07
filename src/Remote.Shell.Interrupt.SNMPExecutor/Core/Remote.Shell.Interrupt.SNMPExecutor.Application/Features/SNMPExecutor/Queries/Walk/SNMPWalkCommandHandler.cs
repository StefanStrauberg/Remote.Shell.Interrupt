namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Queries.Walk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string Oid) : IRequest<JObject>;

internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, JObject>
{
    readonly ISNMPCommandExecutor _executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<JObject> IRequestHandler<SNMPWalkCommand, JObject>.Handle(SNMPWalkCommand request,
                                                                         CancellationToken cancellationToken)
    {
        var response = await _executor.WalkCommand(request.Host,
                                                   request.Community,
                                                   request.Oid,
                                                   cancellationToken);
        return response;
    }
}
