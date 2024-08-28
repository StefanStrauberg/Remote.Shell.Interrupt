namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts;

public interface ISNMPRepository
{
  Task<SNMPResponse> Get(string host,
                         string community,
                         string oid);
  Task<IEnumerable<SNMPResponse>> Walk(string host,
                                       string community,
                                       string oid);
}
