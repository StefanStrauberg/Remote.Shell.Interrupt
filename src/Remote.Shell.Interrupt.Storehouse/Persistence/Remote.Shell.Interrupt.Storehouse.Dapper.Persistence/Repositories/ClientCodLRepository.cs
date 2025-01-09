
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCodLRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<ClientCodL>(postgreSQLDapperContext), IClientCodLRepository
{
  async Task<IEnumerable<ClientCodL>> IClientCodLRepository.GetAllByNameAsync(string name,
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
                "FROM \"ClientCodLs\" " +
                $"WHERE \"Name\" ILIKE '%{name}%' " +
                "AND \"Working\" = true";
    var connection = await postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCodL>(query);

    return result;
  }
}
