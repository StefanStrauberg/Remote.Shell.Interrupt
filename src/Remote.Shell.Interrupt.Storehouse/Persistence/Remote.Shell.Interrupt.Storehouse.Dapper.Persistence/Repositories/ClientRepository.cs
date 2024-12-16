namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository(MySQLDapperContext mySQLDapperContext) : IClientRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCOD>> IClientRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT cc.id_client AS \"Id\", cc.name AS \"Name\", cc.contact_T AS \"Contact\", cc.t_email AS \"Email\", " +
                "GROUP_CONCAT(CAST(vl.id_vlan AS INT) ORDER BY vl.id_vlan) AS \"VLANTags\" " +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS vl ON vl.id_client = cc.id_client " +
                "GROUP BY cc.id_client, cc.name, cc.contact_T, cc.t_email";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCOD>(query);

    return result;
  }

  async Task<ClientCOD> IClientRepository.GetById(int id,
                                                  CancellationToken cancellationToken)
  {
    var query = $"SELECT id_client AS \"Id\", name AS \"Name\", contact_T AS \"Contact\", t_email AS \"Email\" " +
                "FROM client_cod " +
                "WHERE id_client = @Id";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryFirstAsync<ClientCOD>(query, new { Id = id });
    return result;
  }

  async Task<ClientCOD> IClientRepository.GetByVlanTag(int tag,
                                                       CancellationToken cancellationToken)
  {
    var query = $"SELECT cc.id_client AS \"Id\", cc.name AS \"Name\", cc.contact_T AS \"Contact\", cc.t_email AS \"Email\" " +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS sv ON sv.id_client = cc.id_client " +
                "WHERE sv.id_vlan = @Tag";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryFirstAsync<ClientCOD>(query, new { Tag = tag });
    return result;
  }
}
