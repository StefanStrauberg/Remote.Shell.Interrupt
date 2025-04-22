
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

public interface ISPRVlansRepository 
  : IManyQueryRepository<SPRVlan>,
    ICountRepository<SPRVlan>,
    IReadRepository<SPRVlan>,
    IBulkDeleteRepository<SPRVlan>,
    IBulkInsertRepository<SPRVlan>
{ }