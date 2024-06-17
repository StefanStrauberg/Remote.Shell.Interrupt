namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

public interface ISNMPCommandExecutor
{
    Task<IEnumerable<Information>> WalkCommand(string host,
                                               string community,
                                               string oid,
                                               CancellationToken cancellationToken);

    Task<Information> GetCommand(string host,
                                 string community,
                                 string oid,
                                 CancellationToken cancellationToken);
}
