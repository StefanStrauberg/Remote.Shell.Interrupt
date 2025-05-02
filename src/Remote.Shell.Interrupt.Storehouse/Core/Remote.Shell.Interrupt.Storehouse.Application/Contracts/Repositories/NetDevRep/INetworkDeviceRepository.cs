namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for managing NetworkDevice entities, supporting various query and persistence operations.
/// </summary>
public interface INetworkDeviceRepository 
  : IManyQueryRepository<NetworkDevice>,
    IManyQueryWithRelationsRepository<NetworkDevice>,
    IOneQueryWithRelationsRepository<NetworkDevice>,
    IExistenceQueryRepository<NetworkDevice>,
    ICountRepository<NetworkDevice>,
    IInsertRepository<NetworkDevice>,
    IReadRepository<NetworkDevice>
{
  /// <summary>
  /// Deletes a single NetworkDevice entity along with its associated child entities.
  /// </summary>
  /// <param name="networkDeviceToDelete">The NetworkDevice entity to be deleted, including its related children.</param>
  void DeleteOneWithChilren(NetworkDevice networkDeviceToDelete);
}
