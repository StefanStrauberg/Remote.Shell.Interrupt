namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

/// <summary>
/// Defines a repository interface for managing TfPlan entities, supporting various query and persistence operations.
/// </summary>
public interface ITfPlanRepository 
  : IManyQueryRepository<TfPlan>,
    ICountRepository<TfPlan>,
    IReadRepository<TfPlan>,
    IBulkDeleteRepository<TfPlan>,
    IBulkInsertRepository<TfPlan>
{ }