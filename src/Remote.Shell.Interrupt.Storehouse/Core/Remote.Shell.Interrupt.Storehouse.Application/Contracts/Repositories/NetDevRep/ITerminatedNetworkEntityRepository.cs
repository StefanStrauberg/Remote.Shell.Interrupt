namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for bulk inserting TerminatedNetworkEntity records.
/// </summary>
public interface ITerminatedNetworkEntityRepository 
  : IBulkInsertRepository<TerminatedNetworkEntity>
{ }