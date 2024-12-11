namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository(MySQLDapperContext mySQLDapperContext) : IClientRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCOD>> IClientRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT id_client AS \"Id\", name AS \"Name\", contact_T AS \"Contact\", t_email AS \"Email\" FROM client_cod";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCOD>(query);
    return result;
  }

  async Task<ClientCOD> IClientRepository.GetById(int id,
                                                  CancellationToken cancellationToken)
  {
    var query = $"SELECT id_client AS \"Id\", name AS \"Name\", contact_T AS \"Contact\", t_email AS \"Email\" FROM client_cod" +
                "WHERE \"Id\" = @Id";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryFirstAsync<ClientCOD>(query, new { Id = id });
    return result;
  }

  Task<ClientCOD> IClientRepository.GetByVlanTag(int tag,
                                                 CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
