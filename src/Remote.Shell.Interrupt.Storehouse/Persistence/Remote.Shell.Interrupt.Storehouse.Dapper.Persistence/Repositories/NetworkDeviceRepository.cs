using Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class NetworkDeviceRepository(DapperContext context)
  : GenericRepository<NetworkDevice>(context), INetworkDeviceRepository
{
  public Task<IEnumerable<NetworkDevice>> FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<NetworkDevice> FindOneWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
