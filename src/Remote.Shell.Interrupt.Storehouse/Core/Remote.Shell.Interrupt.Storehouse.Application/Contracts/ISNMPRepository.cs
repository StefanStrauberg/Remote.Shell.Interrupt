namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts;

public interface ISNMPRepository
{
  Task<Info> Get(string host, string community, string oid);
  Task<IEnumerable<Info>> Walk(string host, string community, string oid);
}
