namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

public interface ISNMPCommandExecutor
{
    Task<JObject> WalkCommand(string host,
                              string community,
                              string oid,
                              CancellationToken cancellationToken);

    Task<JObject> GetCommand(string host,
                             string community,
                             string oid,
                             CancellationToken cancellationToken);
}
