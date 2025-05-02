namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class ClientsRepository(PostgreSQLDapperContext context,
                                 IAppLogger<ClientsRepository> appLogger,
                                 ICountRepository<Client> countRepository,
                                 IExistenceQueryRepository<Client> existenceQueryRepository,
                                 IManyQueryRepository<Client> manyQueryRepository,
                                 IReadRepository<Client> readRepository,
                                 IBulkDeleteRepository<Client> bulkDeleteRepository,
                                 IBulkInsertRepository<Client> bulkInsertRepository)
  : IClientsRepository
{
  async Task<IEnumerable<Client>> IManyQueryWithRelationsRepository<Client>.GetManyWithChildrenAsync(ISpecification<Client> specification,
                                                                                                     CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<Client>(specification);

    var sql = queryBuilder.Build();

    appLogger.LogInformation(sql);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, Client>();

    await connection.QueryAsync<Client, COD, TfPlan, SPRVlan, Client>(
        sql,
        (cc, c, tf, sprvl) =>
        {
          if (!ccDictionary.TryGetValue(cc.Id, out var client))
          {
            client = cc;
            ccDictionary.Add(client.Id, client);
          }

          if (c is not null && client is not null)
            client.COD = c;

          if (tf is not null && client is not null)
            client.TfPlan = tf;
          
          if (sprvl is not null && client is not null) 
            client.SPRVlans.Add(sprvl);

          return client!;
        },
        splitOn: $"{nameof(Client.Id)},{nameof(COD.Id)},{nameof(TfPlan.Id)},{nameof(SPRVlan.Id)}");

    return ccDictionary.Values;
  }

  async Task<Client> IOneQueryWithRelationsRepository<Client>.GetOneWithChildrenAsync(ISpecification<Client> specification,
                                                                                      CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<Client>(specification);

    var sql = queryBuilder.Build();

    appLogger.LogInformation(sql);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, Client>();

    await connection.QueryAsync<Client, COD, TfPlan, SPRVlan, Client>(
        sql,
        (cc, c, tf, sprvl) =>
        {
          if (!ccDictionary.TryGetValue(cc.Id, out var client))
          {
            client = cc;
            ccDictionary.Add(client.Id, client);
          }

          if (c is not null && client is not null)
            client.COD = c;

          if (tf is not null && client is not null)
            client.TfPlan = tf;
          
          if (sprvl is not null && client is not null) 
            client.SPRVlans.Add(sprvl);

          return client!;
        },
        splitOn: $"{nameof(Client.Id)},{nameof(COD.Id)},{nameof(TfPlan.Id)},{nameof(SPRVlan.Id)}");

    return ccDictionary.Values.First();
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
