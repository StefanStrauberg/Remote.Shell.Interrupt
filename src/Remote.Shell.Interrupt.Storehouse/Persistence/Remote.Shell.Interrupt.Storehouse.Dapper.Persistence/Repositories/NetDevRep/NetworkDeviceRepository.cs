namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class NetworkDeviceRepository(ApplicationDbContext context,
                                       IManyQueryRepository<NetworkDevice> manyQueryRepository,
                                       IExistenceQueryRepository<NetworkDevice> existenceQueryRepository,
                                       ICountRepository<NetworkDevice> countRepository,
                                       IInsertRepository<NetworkDevice> insertRepository,
                                       IReadRepository<NetworkDevice> readRepository) 
  : INetworkDeviceRepository
{
  readonly ApplicationDbContext _context = context;
  readonly DbSet<NetworkDevice> _dbSet = new(context.ModelBuilder, context);

  void INetworkDeviceRepository.DeleteOneWithChilren(NetworkDevice networkDeviceToDelete)
  {
    
  }

  async Task<NetworkDevice> IOneQueryWithRelationsRepository<NetworkDevice>.GetOneWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                    CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Include(x => x.PortsOfNetworkDevice)
                             .Where(specification.Criterias!)
                             .Take(specification.Take)
                             .FirstAsync(connection);
    return result;
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryWithRelationsRepository<NetworkDevice>.GetManyWithChildrenAsync(ISpecification<NetworkDevice> specification,
                                                                                                                   CancellationToken cancellationToken)
  {
    using var connection = await _context.GetConnectionAsync();
    var result = await _dbSet.Include(x => x.PortsOfNetworkDevice)
                             .Where(specification.Criterias!)
                             .Take(specification.Take)
                             .ToListAsync(connection);
    return result;
  }

  async Task<IEnumerable<NetworkDevice>> IManyQueryRepository<NetworkDevice>.GetManyShortAsync(ISpecification<NetworkDevice> specification,
                                                                                               CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);

  async Task<bool> IExistenceQueryRepository<NetworkDevice>.AnyByQueryAsync(ISpecification<NetworkDevice> specification,
                                                                            CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<int> ICountRepository<NetworkDevice>.GetCountAsync(ISpecification<NetworkDevice> specification,
                                                                CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification,
                                           cancellationToken);

  void IInsertRepository<NetworkDevice>.InsertOne(NetworkDevice entity)
    => insertRepository.InsertOne(entity);

  async Task<IEnumerable<NetworkDevice>> IReadRepository<NetworkDevice>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);
}
