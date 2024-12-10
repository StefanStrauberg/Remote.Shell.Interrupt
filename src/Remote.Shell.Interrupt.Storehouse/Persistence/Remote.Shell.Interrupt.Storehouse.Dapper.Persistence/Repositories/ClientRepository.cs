using Remote.Shell.Interrupt.Storehouse.Domain.Organization;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository : IClientRepository
{
  public Task<IEnumerable<ClientCOD>> GetAllAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<ClientCOD> GetById(int id,
                                 CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<ClientCOD> GetByVlanTag(int tag,
                                      CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
