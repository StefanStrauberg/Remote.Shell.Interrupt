namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODLRepository(PostgreSQLDapperContext context) : GenericRepository<ClientCODL>(context), IClientCodLRepository
{
  Task<string?> IClientCodLRepository.GetClientNameByVlanTagAsync(int tag,
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

  async Task<IEnumerable<ClientCODL>> IClientCodLRepository.GetAllByNameAsync(string name,
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
