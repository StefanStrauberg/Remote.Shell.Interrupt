namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientsRepository(PostgreSQLDapperContext context) 
  : GenericRepository<Client>(context), IClientsRepository
{
  async Task<IEnumerable<Client>> IClientsRepository.GetShortClientsByQueryAsync(RequestParameters requestParameters,
                                                                                 CancellationToken cancellationToken)
  {
    var sb = new StringBuilder();
    sb.Append($"SELECT cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", ");
    sb.Append($"cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailT)}\", ");
    sb.Append($"cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.NrDogovor)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");

    var baseQuery = sb.ToString();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "cc",
                                           typeof(Client));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QueryAsync<Client>(finalQuery, parameters);
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetClientsWithChildrensByQueryAsync(RequestParameters requestParameters,
                                                                                         CancellationToken cancellationToken)
  {
    var sb = new StringBuilder();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", ");
    sb.Append($"cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", ");
    sb.Append($"cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", ");
    sb.Append($"cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", ");
    sb.Append($"cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", ");
    sb.Append($"c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", ");
    sb.Append($"c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.DescTfPlan)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.Id)}\", sprvl.\"{nameof(SPRVlan.IdClient)}\", sprvl.\"{nameof(SPRVlan.UseClient)}\", ");
    sb.Append($"sprvl.\"{nameof(SPRVlan.UseCOD)}\", sprvl.\"{nameof(SPRVlan.IdVlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<SPRVlan>()}\" AS sprvl ON sprvl.\"{nameof(SPRVlan.IdClient)}\" = cc.\"{nameof(Client.IdClient)}\" ");

    var baseQuery = sb.ToString();
    var ccDictionary = new Dictionary<Guid, Client>();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "cc",
                                           typeof(Client));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

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
        splitOn: "Id, Id, Id, Id");

    return ccDictionary.Values;
  }
}
