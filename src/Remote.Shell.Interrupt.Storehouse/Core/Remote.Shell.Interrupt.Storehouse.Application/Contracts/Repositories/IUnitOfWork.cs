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
  IClientCODRepository ClientCODRs { get; }
  ICODRRepository CODRs { get; }
  ITfPlanRRepository TfPlanRs { get; }
  IOrganizationRepository Organizations { get; }

  void Complete();
}
