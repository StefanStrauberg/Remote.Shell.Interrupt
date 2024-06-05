namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

public interface ISNMPCommandExecutor
{
    Task<IList<Response>> WalkCommand(string host,
                                      string community,
                                      string oid,
                                      CancellationToken cancellationToken);

    Task<IList<Response>> GetCommand(string host,
                                     string community,
                                     string oid,
                                     CancellationToken cancellationToken);
}
