namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class ClientsRepository(ApplicationDbContext context,
                                 ICountRepository<Client> countRepository,
                                 IExistenceQueryRepository<Client> existenceQueryRepository,
                                 IManyQueryRepository<Client> manyQueryRepository,
                                 IReadRepository<Client> readRepository,
                                 IBulkDeleteRepository<Client> bulkDeleteRepository,
                                 IBulkInsertRepository<Client> bulkInsertRepository)
  : IClientsRepository
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<Client> _dbSet = new(context.ModelBuilder, context);
  
  async Task<IEnumerable<Client>> IManyQueryWithRelationsRepository<Client>.GetManyWithChildrenAsync(ISpecification<Client> specification,
                                                                                                     CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var clients = await _dbSet.Include(x => x.COD)
                              .Include(x => x.TfPlan)
                              .Include(x => x.SPRVlans)
                              .Where(specification.Criterias!)
                              .Take(specification.Take)
                              .ToListAsync(connection);
    return clients;
  }

  async Task<Client> IOneQueryWithRelationsRepository<Client>.GetOneWithChildrenAsync(ISpecification<Client> specification,
                                                                                      CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var clients = await _dbSet.Include(x => x.COD)
                              .Include(x => x.TfPlan)
                              .Include(x => x.SPRVlans)
                              .Where(specification.Criterias!)
                              .FirstAsync(connection);
    return clients;
  }

  async Task<IEnumerable<Client>> IManyQueryRepository<Client>.GetManyShortAsync(ISpecification<Client> specification,
                                                                                 CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);

  async Task<bool> IExistenceQueryRepository<Client>.AnyByQueryAsync(ISpecification<Client> specification,
                                                                     CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<IEnumerable<Client>> IReadRepository<Client>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  void IBulkDeleteRepository<Client>.DeleteMany(IEnumerable<Client> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  void IBulkInsertRepository<Client>.InsertMany(IEnumerable<Client> entities)
    =>  bulkInsertRepository.InsertMany(entities);

  async Task<int> ICountRepository<Client>.GetCountAsync(ISpecification<Client> specification,
                                                         CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification, cancellationToken);
}
