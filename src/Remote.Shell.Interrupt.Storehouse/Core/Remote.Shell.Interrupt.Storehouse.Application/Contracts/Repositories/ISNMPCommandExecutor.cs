namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISNMPCommandExecutor
{
    Task<List<SNMPResponse>> WalkCommand(string host,
                                         string community,
                                         string oid,
                                         CancellationToken cancellationToken,
                                         bool toHex = false,
                                         int repetitions = 20);

    Task<SNMPResponse> GetCommand(string host,
                                  string community,
                                  string oid,
                                  CancellationToken cancellationToken,
                                  bool toHex = false);
}
