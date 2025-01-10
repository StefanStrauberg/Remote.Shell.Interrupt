namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IUnitOfWork
{
  IAssignmentRepository Assignments { get; }
  IBusinessRuleRepository BusinessRules { get; }
  INetworkDeviceRepository NetworkDevices { get; }
  IVLANRepository VLANs { get; }
  IPortRepository Ports { get; }
  IARPEntityRepository ARPEntities { get; }
  IMACEntityRepository MACEntities { get; }
  ITerminatedNetworkEntityRepository TerminatedNetworkEntities { get; }
  IPortVlanRepository PortVlans { get; }
  IClientCODRRepository ClientCODRs { get; }
  IClientCodLRepository ClientCODLs { get; }
  ICODRRepository CODRs { get; }
  ICODLRepository CODLs { get; }
  ITfPlanRRepository TfPlanRs { get; }
  ITfPlanLRepository TfPlanLs { get; }
  ISPRVlanRsRepository SPRVlanRs { get; }
  ISPRVlanLsRepository SPRVlanLs { get; }

  void Complete();
}
