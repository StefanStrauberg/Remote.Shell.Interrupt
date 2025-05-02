namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

/// <summary>
/// Defines a repository interface for managing Client entities, supporting various query and persistence operations.
/// </summary>
public interface IClientsRepository 
  : IOneQueryWithRelationsRepository<Client>,
    IManyQueryWithRelationsRepository<Client>,
    IManyQueryRepository<Client>,
    IExistenceQueryRepository<Client>,
    ICountRepository<Client>,
    IReadRepository<Client>,
    IBulkDeleteRepository<Client>,
    IBulkInsertRepository<Client>
{ }
