namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientsRepository(PostgreSQLDapperContext context) 
  : GenericRepository<Client>(context), IClientsRepository
{
  async Task<IEnumerable<Client>> IClientsRepository.GetAllWithChildrensAsync(CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\"");
    
    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ccDictionary = new Dictionary<Guid, Client>();

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
        splitOn: "Id, Id, Id");

    return [.. ccDictionary.Values];
  }

  async Task<IEnumerable<string>> IClientsRepository.GetClientsNamesByClientIdsAsync(IEnumerable<int> clientId,
                                                                                     CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Name)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"WHERE cc.\"{nameof(Client.IdClient)}\" IN ({JoinIds(clientId)}) ");
    sb.Append($"AND \"{nameof(Client.Working)}\" = true");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<string>(query);

    return result;
  }

  static string JoinIds(IEnumerable<int> ids)
    => string.Join(", ", ids);

  async Task<IEnumerable<Client>> IClientsRepository.GetAllByNameAsync(string name,
                                                                       CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"WHERE cc.\"{nameof(Client.Name)}\" ILIKE '%{name}%' ");
    sb.Append($"AND cc.\"{nameof(Client.Working)}\" = true");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ccDictionary = new Dictionary<Guid, Client>();

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
        splitOn: "Id, Id, Id");

    return [.. ccDictionary.Values];
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetAllByNamesAsync(IEnumerable<string> names,
                                                                        CancellationToken cancellationToken)
  {
    List<Client> clients = [];
    foreach (var name in names)
    {
      clients.AddRange(await ((IClientsRepository)this).GetAllByNameAsync(name, cancellationToken));
    }

    return [.. clients.Distinct()];
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetAllByNameWithChildrensAsync(string name,
                                                                                    CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"WHERE cc.\"{nameof(Client.Name)}\" ILIKE '%{name}%' ");
    sb.Append($"AND cc.\"{nameof(Client.Working)}\" = true");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ccDictionary = new Dictionary<Guid, Client>();

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
        splitOn: "Id, Id, Id");

    return [.. ccDictionary.Values];
  }

  async Task<IEnumerable<Client>> IClientsRepository.GetAllShortAsync(RequestParameters requestParameters,
                                                                             CancellationToken cancellationToken)
  {
    var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
    StringBuilder sb = new();
    sb.Append($"SELECT cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.NrDogovor)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LIMIT {requestParameters.PageSize} OFFSET {offset}");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<Client>(query);

    return result;
  }

  async Task<Client> IClientsRepository.GetClientByIdWithChildrensAsync(Guid id,
                                                                        CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", cc.\"{nameof(Client.Dat1)}\", cc.\"{nameof(Client.Dat2)}\", cc.\"{nameof(Client.Prim1)}\", cc.\"{nameof(Client.Prim2)}\", cc.\"{nameof(Client.Nik)}\", cc.\"{nameof(Client.NrDogovor)}\", ");
    sb.Append($"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", ");
    sb.Append($"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", tf.\"{nameof(TfPlan.DescTfPlan)}\" ");
    sb.Append($"FROM \"{GetTableName<Client>()}\" AS cc ");
    sb.Append($"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" ");
    sb.Append($"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" ");
    sb.Append($"WHERE cc.\"{nameof(Client.Id)}\"=@Id");

    var query = sb.ToString();
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var ccDictionary = new Dictionary<Guid, Client>();

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
}
