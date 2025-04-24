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
  async Task<IEnumerable<Client>> IManyQueryWithRelationsRepository<Client>.GetManyWithChildrenAsync(RequestParameters requestParameters,
                                                                                                     CancellationToken cancellationToken)
  {
    var sb = new StringBuilder();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", ");
    sb.Append($"cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", ");
    sb.Append($"cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", ");
    sb.Append($"cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", ");
    sb.Append($"cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", cc.\"{nameof(Client.Dat1)}\", ");
    sb.Append($"cc.\"{nameof(Client.Dat2)}\", cc.\"{nameof(Client.Prim1)}\", cc.\"{nameof(Client.Prim2)}\", ");
    sb.Append($"cc.\"{nameof(Client.Nik)}\", cc.\"{nameof(Client.NrDogovor)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", ");
    sb.Append($"c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", ");
    sb.Append($"c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.DescTfPlan)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<SPRVlan>()}\" AS sprvl ON sprvl.\"{nameof(SPRVlan.IdClient)}\" = cc.\"{nameof(Client.IdClient)}\"");

    var baseQuery = sb.ToString();
    var ccDictionary = new Dictionary<Guid, Client>();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "cc",
                                           typeof(Client));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    await connection.QueryAsync<Client, COD, TfPlan, SPRVlan, Client>(
        finalQuery,
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
            client.TfPlanL = tf;
          
          if (sprvl is not null && client is not null) 
            client.SPRVlans.Add(sprvl);

          return client!;
        },
        parameters,
        splitOn: $"{nameof(Client.Id)}, {nameof(COD.Id)}, {nameof(TfPlan.Id)}, {nameof(SPRVlan.Id)}");

    return ccDictionary.Values;
  }

  async Task<Client> IOneQueryWithRelationsRepository<Client>.GetOneWithChildrensAsync(RequestParameters requestParameters,
                                                                                       CancellationToken cancellationToken)
  {
    var sb = new StringBuilder();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", ");
    sb.Append($"cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", ");
    sb.Append($"cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", ");
    sb.Append($"cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", ");
    sb.Append($"cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", cc.\"{nameof(Client.Dat1)}\", ");
    sb.Append($"cc.\"{nameof(Client.Dat2)}\", cc.\"{nameof(Client.Prim1)}\", cc.\"{nameof(Client.Prim2)}\", ");
    sb.Append($"cc.\"{nameof(Client.Nik)}\", cc.\"{nameof(Client.NrDogovor)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", ");
    sb.Append($"c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", ");
    sb.Append($"c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.DescTfPlan)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName.Handle<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName.Handle<SPRVlan>()}\" AS sprvl ON sprvl.\"{nameof(SPRVlan.IdClient)}\" = cc.\"{nameof(Client.IdClient)}\"");

    var baseQuery = sb.ToString();
    var ccDictionary = new Dictionary<Guid, Client>();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "cc",
                                           typeof(Client));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery, true);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    await connection.QueryAsync<Client, COD, TfPlan, SPRVlan, Client>(
        finalQuery,
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
            client.TfPlanL = tf;
          
          if (sprvl is not null && client is not null) 
            client.SPRVlans.Add(sprvl);

          return client!;
        },
        parameters,
        splitOn: $"{nameof(Client.Id)}, {nameof(COD.Id)}, {nameof(TfPlan.Id)}, {nameof(SPRVlan.Id)}");

    return ccDictionary.Values.First();
  }

  async Task<IEnumerable<Client>> IManyQueryRepository<Client>.GetManyShortAsync(RequestParameters requestParameters,
                                                                                 CancellationToken cancellationToken,
                                                                                 bool skipFiltering)
    => await manyQueryRepository.GetManyShortAsync(requestParameters,
                                                   cancellationToken,
                                                   skipFiltering);

  async Task<bool> IExistenceQueryRepository<Client>.AnyByQueryAsync(RequestParameters requestParameters,
                                                                     CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(requestParameters,
                                                      cancellationToken);

  async Task<int> ICountRepository<Client>.GetCountAsync(RequestParameters requestParameters,
                                                         CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(requestParameters,
                                           cancellationToken);

  async Task<IEnumerable<Client>> IReadRepository<Client>.GetAllAsync(CancellationToken cancellationToken)
    => await readRepository.GetAllAsync(cancellationToken);

  void IBulkDeleteRepository<Client>.DeleteMany(IEnumerable<Client> entities)
    => bulkDeleteRepository.DeleteMany(entities);

  void IBulkInsertRepository<Client>.InsertMany(IEnumerable<Client> entities)
    =>  bulkInsertRepository.InsertMany(entities);
}
