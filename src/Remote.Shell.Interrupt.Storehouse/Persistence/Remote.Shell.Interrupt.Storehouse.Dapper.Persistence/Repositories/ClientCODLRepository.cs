namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODLRepository(PostgreSQLDapperContext context) : GenericRepository<ClientCODL>(context), IClientCODLRepository
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

    return ccDictionary.Values.ToList();
  }

  async Task<string?> IClientCODLRepository.GetClientNameByClientIdAsync(int clientId,
                                                                         CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Name\" " +
                "FROM \"ClientCODLs\" AS cc " +
                $"WHERE cc.\"IdClient\" = '{clientId}' " +
                "AND \"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.ExecuteScalarAsync<string>(query);

    return result;
  }

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

    return ccDictionary.Values.ToList();
  }

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

    return ccDictionary.Values.ToList();
  }
}
