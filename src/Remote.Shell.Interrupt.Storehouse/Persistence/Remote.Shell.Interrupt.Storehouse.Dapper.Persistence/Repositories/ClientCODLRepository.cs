namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODLRepository(PostgreSQLDapperContext context) : GenericRepository<ClientCODL>(context), IClientCODLRepository
{
  public async Task<IEnumerable<CODL>> GetAllAsync(System.Threading.CancellationToken cancellationToken)
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
    var result = await connection.QueryAsync<CODL>(query);

    return result;
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
