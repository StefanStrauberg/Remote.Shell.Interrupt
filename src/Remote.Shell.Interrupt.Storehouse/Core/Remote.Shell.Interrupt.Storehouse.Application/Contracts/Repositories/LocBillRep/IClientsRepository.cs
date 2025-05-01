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
  Task<Client> GetOneWithChildrensAsync(ISpecification<Client> specification,
                                        CancellationToken cancellationToken);

  Task<IEnumerable<Client>> GetManyShortAsync(ISpecification<Client> specification,
                                              CancellationToken cancellationToken);

  Task<IEnumerable<Client>> GetManyWithChildrenAsync(ISpecification<Client> specification,
                                                     CancellationToken cancellationToken);

  Task<int> GetCountAsync(ISpecification<Client> specification,
                          CancellationToken cancellationToken);

  Task<bool> AnyByQueryAsync(ISpecification<Client> specification,
                             CancellationToken cancellationToken);
}
