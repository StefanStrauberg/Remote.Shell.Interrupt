namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface INetworkDeviceRepository : IGenericRepository<NetworkDevice>
{
  Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken);
  Task<NetworkDevice> GetFirstWithChildrensByIdAsync(Guid id,
                                                     CancellationToken cancellationToken);

  Task<IEnumerable<NetworkDevice>> FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression,
                                                             CancellationToken cancellationToken);
}
