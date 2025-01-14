namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ISPRVlanLsRepository : IGenericRepository<SPRVlanL>
{
  Task<bool> AnyByVlanTagAsync(int vlanTag,
                               CancellationToken cancellationToken);
  Task<IEnumerable<SPRVlanL>> GetAllByIdsAsync(IEnumerable<int> ids,
                                             CancellationToken cancellationToken);
  Task<SPRVlanL> GetByVlanTagAsync(int id,
                                   CancellationToken cancellationToken);
}