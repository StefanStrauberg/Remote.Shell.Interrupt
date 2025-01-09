
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IClientCodLRepository : IGenericRepository<ClientCodL>
{
  Task<IEnumerable<ClientCodL>> GetAllByNameAsync(string name,
                                                  CancellationToken cancellationToken);
}
