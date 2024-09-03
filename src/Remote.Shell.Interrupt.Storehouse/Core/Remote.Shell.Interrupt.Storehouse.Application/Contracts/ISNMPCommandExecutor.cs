namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts;

public interface ISNMPCommandExecutor
{
    Task<List<SNMPResponse>> WalkCommand(string host,
                                         string community,
                                         string oid,
                                         CancellationToken cancellationToken);

    Task<SNMPResponse> GetCommand(string host,
                                 string community,
                                 string oid,
                                 CancellationToken cancellationToken);
}
