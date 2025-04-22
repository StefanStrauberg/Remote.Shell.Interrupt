namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

public interface ITfPlanRepository 
  : IManyQueryRepository<TfPlan>,
    ICountRepository<TfPlan>,
    IReadRepository<TfPlan>,
    IBulkDeleteRepository<TfPlan>,
    IBulkInsertRepository<TfPlan>
{ }