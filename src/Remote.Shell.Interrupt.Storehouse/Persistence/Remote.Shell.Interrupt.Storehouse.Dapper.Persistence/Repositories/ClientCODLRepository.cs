using Remote.Shell.Interrupt.Storehouse.Application.Helper;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODLRepository(PostgreSQLDapperContext context) 
  : GenericRepository<ClientCODL>(context), IClientCODLRepository
{
  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllWithChildrensAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                "FROM \"ClientCODLs\" AS cc " +
                "LEFT JOIN \"CODLs\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                "LEFT JOIN \"TfPlanLs\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\"";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, ClientCODL>();

    await connection.QueryAsync<ClientCODL, CODL, TfPlanL, ClientCODL>(
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

  async Task<IEnumerable<string>> IClientCODLRepository.GetClientsNamesByClientIdsAsync(IEnumerable<int> clientId,
                                                                                        CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Name\" " +
                "FROM \"ClientCODLs\" AS cc " +
                $"WHERE cc.\"IdClient\" IN ({JoinIds(clientId)}) " +
                "AND \"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<string>(query);

    return result;
  }

  static string JoinIds(IEnumerable<int> ids)
    => string.Join(", ", ids);

  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllByNameAsync(string name,
                                                                              CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                "FROM \"ClientCODLs\" AS cc " +
                "LEFT JOIN \"CODLs\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                "LEFT JOIN \"TfPlanLs\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\" " +
                $"WHERE cc.\"Name\" ILIKE '%{name}%' " +
                "AND cc.\"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, ClientCODL>();

    await connection.QueryAsync<ClientCODL, CODL, TfPlanL, ClientCODL>(
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

  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllByNamesAsync(IEnumerable<string> names,
                                                                               CancellationToken cancellationToken)
  {
    List<ClientCODL> clients = [];
    foreach (var name in names)
    {
      clients.AddRange(await ((IClientCODLRepository)this).GetAllByNameAsync(name, cancellationToken));
    }

    return [.. clients.Distinct()];
  }

  static string JoinNames(IEnumerable<string> names)
    => string.Join(", ", names);

  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllByNameWithChildrensAsync(string name,
                                                                                           CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactC\", cc.\"TelephoneC\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailC\", cc.\"Working\", cc.\"EmailT\", cc.\"History\", cc.\"AntiDDOS\", cc.\"Id_COD\", cc.\"Id_TfPlan\", " +
                "c.\"Id\", c.\"IdCOD\", c.\"NameCOD\", c.\"Telephone\", c.\"Email1\", c.\"Email2\", c.\"Contact\", c.\"Description\", c.\"Region\", " +
                "tf.\"Id\", tf.\"IdTfPlan\", tf.\"NameTfPlan\", tf.\"DescTfPlan\" " +
                "FROM \"ClientCODLs\" AS cc " +
                "LEFT JOIN \"CODLs\" AS c ON c.\"IdCOD\" = cc.\"Id_COD\" " +
                "LEFT JOIN \"TfPlanLs\" AS tf ON tf.\"IdTfPlan\" = cc.\"Id_TfPlan\" " +
                $"WHERE cc.\"Name\" ILIKE '%{name}%' " +
                "AND cc.\"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, ClientCODL>();

    await connection.QueryAsync<ClientCODL, CODL, TfPlanL, ClientCODL>(
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

  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllShortAsync(RequestParameters requestParameters,
                                                                             CancellationToken cancellationToken)
  {
    var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
    var query = $"SELECT cc.\"Id\", cc.\"IdClient\", cc.\"Name\", cc.\"ContactT\", cc.\"TelephoneT\", cc.\"EmailT\", cc.\"Working\", cc.\"AntiDDOS\" " +
                "FROM \"ClientCODLs\" AS cc " +
                $"LIMIT {requestParameters.PageSize} OFFSET {offset}";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCODL>(query);

    return result;
  }
}
