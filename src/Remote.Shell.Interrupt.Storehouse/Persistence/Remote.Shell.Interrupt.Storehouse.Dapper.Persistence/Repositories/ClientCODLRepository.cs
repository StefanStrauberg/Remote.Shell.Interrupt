namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODLRepository(PostgreSQLDapperContext context) : GenericRepository<ClientCODL>(context), IClientCODLRepository
{
  async Task<IEnumerable<ClientCODL>> IGenericRepository<ClientCODL>.GetAllAsync(System.Threading.CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.\"Id\", " +
                "cc.\"IdClient\", " +
                "cc.\"Name\", " +
                "cc.\"ContactC\", " +
                "cc.\"TelephoneC\", " +
                "cc.\"ContactT\", " +
                "cc.\"TelephoneT\", " +
                "cc.\"EmailC\", " +
                "cc.\"Working\", " +
                "cc.\"EmailT\", " +
                "cc.\"History\", " +
                "cc.\"AntiDDOS\", " +
                "cc.\"IdCOD\", " +
                "cc.\"IdTPlan\", " +
                "c.\"Id\", " +
                "c.\"IdCOD\", " +
                "c.\"NameCOD\", " +
                "c.\"Telephone\", " +
                "c.\"Email1\", " +
                "c.\"Email2\", " +
                "c.\"Contact\", " +
                "c.\"Description\", " +
                "c.\"Region\", " +
                "tf.\"Id\", " +
                "tf.\"IdTfPlan\", " +
                "tf.\"NameTfPlan\", " +
                "tf.\"DescTfPlan\" " +
                "FROM \"ClientCODLs\" AS cc " +
                "LEFT JOIN \"CODLs\" AS c ON c.\"Id\" = cc.\"IdCOD\" " +
                "LEFT JOIN \"TfPlanLs\" AS tf ON tf.\"Id\" = cc.\"IdTPlan\"";
    var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<Guid, ClientCODL>();
    var cDicotionary = new Dictionary<Guid, CODL>();
    var tfDictionary = new Dictionary<Guid, TfPlanL>();

    await connection.QueryAsync<ClientCODL, CODL, TfPlanL, ClientCODL>(
        query,
        (cc, c, tf) =>
        {
          if (!ccDictionary.TryGetValue(cc.Id, out var ClientCODL))
          {
            ClientCODL = cc;
            ccDictionary.Add(ClientCODL.Id, ClientCODL);
          }

          if (!cDicotionary.TryGetValue(c.Id, out var CODL))
          {
            CODL = c;
            ClientCODL.COD = c;
            cDicotionary.Add(CODL.Id, CODL);
          }

          if (!tfDictionary.TryGetValue(tf.Id, out var tfPlanL))
          {
            tfPlanL = tf;
            ClientCODL.TfPlanL = tfPlanL;
            tfDictionary.Add(tfPlanL.Id, tfPlanL);
          }

          return ClientCODL;
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
                "\"IdCOD\", " +
                "\"IdTPlan\", " +
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
