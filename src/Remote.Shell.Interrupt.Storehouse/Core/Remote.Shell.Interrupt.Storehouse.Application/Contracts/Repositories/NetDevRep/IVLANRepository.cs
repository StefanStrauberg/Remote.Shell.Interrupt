namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for bulk inserting VLAN records.
/// </summary>
public interface IVLANRepository 
  : IBulkInsertRepository<VLAN>
{ }
