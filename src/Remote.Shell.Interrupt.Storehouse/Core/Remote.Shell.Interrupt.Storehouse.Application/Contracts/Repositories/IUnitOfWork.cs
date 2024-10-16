namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IUnitOfWork
{
  IAssignmentRepository Assignments { get; }
  IBusinessRuleRepository BusinessRules { get; }
  INetworkDeviceRepository NetworkDevices { get; }
  IVLANRepository VLANs { get; }
  Task CompleteAsync(CancellationToken cancellationToken);
}
