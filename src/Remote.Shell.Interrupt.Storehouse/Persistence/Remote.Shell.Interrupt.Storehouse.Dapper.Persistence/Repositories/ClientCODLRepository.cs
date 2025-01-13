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
    var cDicotionary = new Dictionary<Guid, CODL>();
    var tfDictionary = new Dictionary<Guid, TfPlanL>();

    await connection.QueryAsync<ClientCODL, CODL, TfPlanL, ClientCODL>(
        query,
        (cc, c, tf) =>
        {
          if (!ccDictionary.TryGetValue(cc.Id, out var clientCODL))
          {
            clientCODL = cc;
            ccDictionary.Add(clientCODL.Id, clientCODL);
          }

          if (!cDicotionary.TryGetValue(c.Id, out var codL))
          {
            codL = c;
            clientCODL.COD = c;
            cDicotionary.Add(codL.Id, codL);
          }

          if (tf is not null && !tfDictionary.TryGetValue(tf.Id, out var tfPlanL))
          {
            tfPlanL = tf;
            clientCODL.TfPlanL = tf;
            tfDictionary.Add(codL.Id, tfPlanL);
          }

          return clientCODL;
        },
        splitOn: "Id, Id, Id");

    return ccDictionary.Values.ToList();
  }

  Task<string?> IClientCODLRepository.GetClientNameByVlanTagAsync(int tag,
                                                                  CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
    // var query = $"SELECT " +
    //             "\"Name\" " +
    //             "FROM \"ClientCodLs\" " +
    //             $"WHERE \"Name\" ILIKE '%{name}%' " +
    //             "AND \"Working\" = true";
    // var connection = await postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    // var result = await connection.QueryAsync<ClientCODL>(query);

    // return result;
  }

  async Task<IEnumerable<ClientCODL>> IClientCODLRepository.GetAllByNameAsync(string name,
                                                                            CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "\"IdClient\", " +
                "\"Name\", " +
                "\"ContactC\", " +
                "\"TelephoneC\", " +
                "\"ContactT\", " +
                "\"TelephoneT\", " +
                "\"EmailC\", " +
                "\"Working\", " +
                "\"EmailT\", " +
                "\"Id_COD\", " +
                "\"Id_TfPlan\", " +
                "\"History\", " +
                "\"AntiDDOS\" " +
                "FROM \"ClientCODLs\" " +
                $"WHERE \"Name\" ILIKE '%{name}%' " +
                "AND \"Working\" = true";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCODL>(query);

    return result;
  }
}
