namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;

/// <summary>
/// Defines a repository interface for bulk inserting MACEntity records.
/// </summary>
public interface IMACEntityRepository
  : IBulkInsertRepository<MACEntity>
{ }
