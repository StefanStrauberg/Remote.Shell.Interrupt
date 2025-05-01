namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.LocBillRep;

internal class ClientsRepository(PostgreSQLDapperContext context,
                                 ICountRepository<Client> countRepository,
                                 IExistenceQueryRepository<Client> existenceQueryRepository,
                                 IManyQueryRepository<Client> manyQueryRepository,
                                 IReadRepository<Client> readRepository,
                                 IBulkDeleteRepository<Client> bulkDeleteRepository,
                                 IBulkInsertRepository<Client> bulkInsertRepository)
  : IClientsRepository
{
  async Task<IEnumerable<Client>> IClientsRepository.GetManyShortAsync(ISpecification<Client> specification,
                                                                       CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilderUpdated<Client>(specification);

    var sql = queryBuilder.Build();

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<Client>(sql);
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetManyWithChildrenAsync(ISpecification<Client> specification,
                                                                              CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilderUpdated<Client>(specification);

    var sql = queryBuilder.Build();

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

  async Task<Client> IClientsRepository.GetOneWithChildrensAsync(ISpecification<Client> specification,
                                                                 CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilderUpdated<Client>(specification);

    var sql = queryBuilder.Build();
    
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

  Task<IEnumerable<Client>> IManyQueryRepository<Client>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                 CancellationToken cancellationToken,
                                                                                 bool skipFiltering)
  {
    throw new NotImplementedException();
  }

  Task<bool> IExistenceQueryRepository<Client>.AnyByQueryAsync(RequestParameters requestParameters,
                                                               CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  async Task<int> IClientsRepository.GetCountAsync(ISpecification<Client> specification,
                                                   CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilderUpdated<Client>(specification);

    var sql = queryBuilder.BuildCount();

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.ExecuteScalarAsync<int>(sql);
  }

  async Task<bool> IClientsRepository.AnyByQueryAsync(ISpecification<Client> specification,
                                                      CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilderUpdated<Client>(specification);

    var sql = queryBuilder.BuildCount();

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.ExecuteScalarAsync<int>(sql) > 0;
  }

  async Task<IEnumerable<Client>> IReadRepository<Client>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  void IBulkDeleteRepository<Client>.DeleteMany(IEnumerable<Client> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  void IBulkInsertRepository<Client>.InsertMany(IEnumerable<Client> entities)
    =>  bulkInsertRepository.InsertMany(entities);

  public Task<IEnumerable<Client>> GetManyWithChildrenAsync(RequestParameters requestParameters, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<int> GetCountAsync(RequestParameters requestParameters, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<Client> GetOneWithChildrensAsync(RequestParameters requestParameters, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
