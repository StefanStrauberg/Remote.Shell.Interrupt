namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

public interface INetworkDeviceRepository 
  : IManyQueryRepository<NetworkDevice>,
    IManyQueryWithRelationsRepository<NetworkDevice>,
    IOneQueryWithRelationsRepository<NetworkDevice>,
    IExistenceQueryRepository<NetworkDevice>,
    ICountRepository<NetworkDevice>,
    IInsertRepository<NetworkDevice>,
    IReadRepository<NetworkDevice>
{
  void DeleteOneWithChilren(NetworkDevice networkDeviceToDelete);
}
