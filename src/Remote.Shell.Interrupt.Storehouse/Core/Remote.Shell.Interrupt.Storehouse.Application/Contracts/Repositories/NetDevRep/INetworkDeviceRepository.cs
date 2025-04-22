namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

public interface INetworkDeviceRepository
{
  void DeleteOneWithChilren(NetworkDevice networkDeviceToDelete);
}
