namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

public interface ISNMPCommandExecutor
{
    Task<JsonArray> WalkCommand(string host,
                                string community,
                                string oid,
                                CancellationToken cancellationToken);

    Task<JsonObject> GetCommand(string host,
                                string community,
                                string oid,
                                CancellationToken cancellationToken);
}
