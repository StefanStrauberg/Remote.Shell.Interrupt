namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientsRepository(PostgreSQLDapperContext context) 
  : GenericRepository<Client>(context), IClientsRepository
{
  async Task<IEnumerable<Client>> IClientsRepository.GetAllShortClientsAsync(RequestParameters requestParameters,
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
    var result = await connection.QueryAsync<Client>(finalQuery, parameters);

    return result;
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetAllClientsWithChildrensAsync(RequestParameters requestParameters,
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
    sb.Append($"tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");

    var baseQuery = sb.ToString();
    var queryBuilder = new SqlQueryBuilder(requestParameters,
                                           "cc",
                                           typeof(Client));
    var (finalQuery, parameters) = queryBuilder.BuildBaseQuery(baseQuery);
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<Client>(finalQuery, parameters);

    return result;
  }

  async Task<Client> IClientsRepository.GetClientByIdWithChildrensAsync(Guid id,
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
    sb.Append($"tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"WHERE cc.\"{nameof(Client.Id)}\"=@Id");

    var query = sb.ToString();
    var ccDictionary = new Dictionary<Guid, Client>();
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    await connection.QueryAsync<Client, COD, TfPlan, Client>(
        query,
        (cc, c, tf) =>
        {
          var clientCODL = cc;
          ccDictionary.Add(cc.Id, clientCODL);

          if (c is not null)
            clientCODL.COD = c;

          if (tf is not null)
            clientCODL.TfPlanL = tf;

          return clientCODL;
        },
        new { Id = id },
        splitOn: "Id, Id, Id");

    return ccDictionary.Values.First();
  }

  async Task<Client> IClientsRepository.GetShortClientByIdAsync(Guid id,
                                                                CancellationToken cancellationToken)
  {
    var sb = new StringBuilder();
    sb.Append("SELECT ");
    sb.Append($"SELECT cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", ");
    sb.Append($"cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailT)}\", ");
    sb.Append($"cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.NrDogovor)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"WHERE cc.\"{nameof(Client.Id)}\"=@Id");

    var baseQuery = sb.ToString();
    
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QuerySingleAsync<Client>(baseQuery, new { Id = id });

    return result;
  }
}
