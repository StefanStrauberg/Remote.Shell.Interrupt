namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

/// <summary>
/// Defines a repository interface for managing COD entities, supporting various query and persistence operations.
/// </summary>
public interface ICODRepository
  : IReadRepository<COD>,
    IBulkDeleteRepository<COD>,
    IBulkInsertRepository<COD>
{ }
