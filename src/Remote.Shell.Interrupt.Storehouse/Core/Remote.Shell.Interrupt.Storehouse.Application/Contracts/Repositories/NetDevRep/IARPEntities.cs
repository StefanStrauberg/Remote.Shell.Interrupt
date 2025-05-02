namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for bulk inserting ARPEntity records.
/// </summary>
public interface IARPEntityRepository
  : IBulkInsertRepository<ARPEntity>
{ }