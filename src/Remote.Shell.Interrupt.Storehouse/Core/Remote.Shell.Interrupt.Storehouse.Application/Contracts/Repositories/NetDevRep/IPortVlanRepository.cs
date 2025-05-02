namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for bulk inserting PortVlan records.
/// </summary>
public interface IPortVlanRepository
  : IBulkInsertRepository<PortVlan>
{ }