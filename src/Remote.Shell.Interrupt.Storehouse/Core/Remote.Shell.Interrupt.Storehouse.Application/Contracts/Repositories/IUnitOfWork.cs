namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IUnitOfWork
{
  INetworkDeviceRepository NetworkDevices { get; }
  IVLANRepository VLANs { get; }
  IPortRepository Ports { get; }
  IARPEntityRepository ARPEntities { get; }
  IMACEntityRepository MACEntities { get; }
  ITerminatedNetworkEntityRepository TerminatedNetworkEntities { get; }
  IPortVlanRepository PortVlans { get; }
  IRemoteClientsRepository RemoteClients { get; }
  IClientsRepository Clients { get; }
  IRemoteCODRepository RemoteCODs { get; }
  ICODRepository CODs { get; }
  IRemoteTfPlanRepository RemoteTfPlans { get; }
  ITfPlanRepository TfPlans { get; }
  IRemoteSPRVlansRepository RemoteSPRVlans { get; }
  ISPRVlansRepository SPRVlans { get; }
  IGateRepository GateRepository { get; }

  void Complete();
}
