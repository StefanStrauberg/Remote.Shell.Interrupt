namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

public interface ICODRepository
  : IReadRepository<COD>,
    IBulkDeleteRepository<COD>,
    IBulkInsertRepository<COD>
{ }
