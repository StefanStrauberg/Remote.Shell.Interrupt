namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientRepository
{
  Task<IEnumerable<ClientCOD>> GetAllAsync(CancellationToken cancellationToken);
  Task<ClientCOD> GetById(int id,
                          CancellationToken cancellationToken);
  Task<ClientCOD> GetByVlanTag(int tag,
                               CancellationToken cancellationToken);
}
