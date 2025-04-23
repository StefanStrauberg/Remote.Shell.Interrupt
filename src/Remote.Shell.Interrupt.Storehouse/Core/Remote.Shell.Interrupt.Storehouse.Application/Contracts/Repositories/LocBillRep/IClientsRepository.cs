using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Specification;

namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;

public interface IClientsRepository 
  : IOneQueryWithRelationsRepository<Client>,
    IManyQueryWithRelationsRepository<Client>,
    IManyQueryRepository<Client>,
    IExistenceQueryRepository<Client>,
    ICountRepository<Client>,
    IReadRepository<Client>,
    IBulkDeleteRepository<Client>,
    IBulkInsertRepository<Client>
{ 
  Task<IEnumerable<Client>> GetManyWithChildrenAsync(ISpecification<Client> specification,
                                                     CancellationToken cancellationToken);
}
