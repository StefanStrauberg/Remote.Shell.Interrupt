namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientsRepository(PostgreSQLDapperContext context) 
  : GenericRepository<Client>(context), IClientsRepository
{
  async Task<IEnumerable<Client>> IClientsRepository.GetAllWithChildrensAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                $"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\"";
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
    var query = $"SELECT " +
                "cc.\"Name\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"WHERE cc.\"IdClient\" IN ({JoinIds(clientId)}) " +
                "AND \"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<string>(query);

    return result;
  }

  static string JoinIds(IEnumerable<int> ids)
    => string.Join(", ", ids);

  async Task<IEnumerable<Client>> IClientsRepository.GetAllByNameAsync(string name,
                                                                              CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                $"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\" " +
                $"WHERE cc.\"Name\" ILIKE '%{name}%' " +
                "AND cc.\"Working\" = true";
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

  static string AddPercentSigns(string input)
  {
    StringBuilder sb = new();

    sb.Append('%');
    sb.Append(input);
    sb.Append('%');

    return sb.ToString();
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

  static string JoinNames(IEnumerable<string> names)
    => string.Join(", ", names);

  async Task<IEnumerable<Client>> IClientsRepository.GetAllByNameWithChildrensAsync(string name,
                                                                                           CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                $"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\" " +
                $"WHERE cc.\"Name\" ILIKE '%{name}%' " +
                "AND cc.\"Working\" = true";
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
    var query = $"SELECT cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailT\", cc.\"Working\", cc.\"AntiDDOS\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"LIMIT {requestParameters.PageSize} OFFSET {offset}";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<Client>(query);

    return result;
  }

  async Task<Client> IClientsRepository.GetClientByIdWithChildrensAsync(Guid id,
                                                                        CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                $"cc.\"{nameof(Client.Id)}\", cc.\"{nameof(Client.IdClient)}\", cc.\"{nameof(Client.Name)}\", cc.\"{nameof(Client.ContactC)}\", cc.\"{nameof(Client.TelephoneC)}\", cc.\"{nameof(Client.ContactT)}\", cc.\"{nameof(Client.TelephoneT)}\", cc.\"{nameof(Client.EmailC)}\", cc.\"{nameof(Client.Working)}\", cc.\"{nameof(Client.EmailT)}\", cc.\"{nameof(Client.History)}\", cc.\"{nameof(Client.AntiDDOS)}\", cc.\"{nameof(Client.Id_COD)}\", cc.\"{nameof(Client.Id_TfPlan)}\", " +
                $"c.\"{nameof(COD.Id)}\", c.\"{nameof(COD.IdCOD)}\", c.\"{nameof(COD.NameCOD)}\", c.\"{nameof(COD.Telephone)}\", c.\"{nameof(COD.Email1)}\", c.\"{nameof(COD.Email2)}\", c.\"{nameof(COD.Contact)}\", c.\"{nameof(COD.Description)}\", c.\"{nameof(COD.Region)}\", " +
                $"tf.\"{nameof(TfPlan.Id)}\", tf.\"{nameof(TfPlan.IdTfPlan)}\", tf.\"{nameof(TfPlan.NameTfPlan)}\", tf.\"{nameof(TfPlan.DescTfPlan)}\" " +
                $"FROM \"{GetTableName<Client>()}\" AS cc " +
                $"LEFT JOIN \"{GetTableName<COD>()}\" AS c ON c.\"{nameof(COD.IdCOD)}\" = cc.\"{nameof(Client.Id_COD)}\" " +
                $"LEFT JOIN \"{GetTableName<TfPlan>()}\" AS tf ON tf.\"{nameof(TfPlan.IdTfPlan)}\" = cc.\"{nameof(Client.Id_TfPlan)}\" " +
                $"WHERE cc.\"{nameof(Client.Id)}\"=@Id";
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
