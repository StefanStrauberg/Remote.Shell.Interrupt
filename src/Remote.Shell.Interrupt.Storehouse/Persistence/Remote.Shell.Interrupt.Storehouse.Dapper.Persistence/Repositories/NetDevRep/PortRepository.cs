namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.NetDevRep;

internal class PortRepository(ApplicationDbContext context,
                              IExistenceQueryRepository<Port> existenceQueryRepository,
                              IOneQueryRepository<Port> oneQueryRepository,
                              IBulkInsertRepository<Port> bulkInsertRepository,
                              IBulkDeleteRepository<Port> bulkDeleteRepository, 
                              IBulkReplaceRepository<Port> bulkReplaceRepository)
  : IPortRepository
{
  async Task<IEnumerable<Port>> IPortRepository.GetAllAggregatedPortsByListAsync(IEnumerable<Guid> Ids,
                                                                                 CancellationToken cancellationToken)
  {
    var result = await context.Set<Port>()
                              .AsNoTracking()
                              .Where(p => Ids.ToList().Contains(p.ParentId!.Value))
                              .ToListAsync(cancellationToken);
    return result;
  }

  Task<string> IPortRepository.LookingForInterfaceNameByIPAsync(string ipAddress,
                                                                      CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  Task<IEnumerable<Port>> IPortRepository.GetPortsWithMacAddressesAndSpecificHostsAsync(string MACAddress,
                                                                                              List<string> hosts,
                                                                                              CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  async Task<bool> IExistenceQueryRepository<Port>.AnyByQueryAsync(ISpecification<Port> specification,
                                                                   CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<Port> IOneQueryRepository<Port>.GetOneShortAsync(ISpecification<Port> specification,
                                                              CancellationToken cancellationToken)
    => await oneQueryRepository.GetOneShortAsync(specification,
                                                 cancellationToken);

  void IBulkInsertRepository<Port>.InsertMany(IEnumerable<Port> entities)
    => bulkInsertRepository.InsertMany(entities);

  void IBulkDeleteRepository<Port>.DeleteMany(IEnumerable<Port> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  void IBulkReplaceRepository<Port>.ReplaceMany(IEnumerable<Port> entities)
    => bulkReplaceRepository.ReplaceMany(entities);
}