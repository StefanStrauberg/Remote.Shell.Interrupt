namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

public interface INetDevUnitOfWork
{
  INetworkDeviceRepository NetworkDevices { get; }
  IVLANRepository VLANs { get; }
  IPortRepository Ports { get; }
  IARPEntityRepository ARPEntities { get; }
  IMACEntityRepository MACEntities { get; }
  ITerminatedNetworkEntityRepository TerminatedNetworkEntities { get; }
  IPortVlanRepository PortVlans { get; }
  void Complete();
}