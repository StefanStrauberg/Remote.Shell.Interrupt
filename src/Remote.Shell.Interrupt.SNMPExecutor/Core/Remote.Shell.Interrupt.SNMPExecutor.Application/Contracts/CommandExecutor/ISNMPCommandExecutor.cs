namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

public interface ISNMPCommandExecutor
{
    Task<IEnumerable<Info>> WalkCommand(string host,
                                               string community,
                                               string oid,
                                               CancellationToken cancellationToken);

    Task<Info> GetCommand(string host,
                                 string community,
                                 string oid,
                                 CancellationToken cancellationToken);
}
