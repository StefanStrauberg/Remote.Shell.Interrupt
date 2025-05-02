namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

/// <summary>
/// Defines a repository interface for managing SPRVlan entities, supporting various query and persistence operations.
/// </summary>
public interface ISPRVlansRepository 
  : IManyQueryRepository<SPRVlan>,
    ICountRepository<SPRVlan>,
    IReadRepository<SPRVlan>,
    IBulkDeleteRepository<SPRVlan>,
    IBulkInsertRepository<SPRVlan>
{ }