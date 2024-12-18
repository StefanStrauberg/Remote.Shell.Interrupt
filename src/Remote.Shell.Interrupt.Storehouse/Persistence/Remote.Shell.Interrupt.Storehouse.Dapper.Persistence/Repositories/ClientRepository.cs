namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository(MySQLDapperContext mySQLDapperContext) : IClientRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCOD>> IClientRepository.GetAllByNameAsync(string name,
                                                                         CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.id_client AS \"Id\", " +
                "cc.name AS \"Name\", " +
                "cc.telefon_T AS \"Contact\", " +
                "cc.t_email AS \"Email\", " +
                "tp.name_tplan AS \"TPlan\", " +
                "GROUP_CONCAT(CAST(vl.id_vlan AS INT) ORDER BY vl.id_vlan) AS \"VLANTags\" " +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS vl ON vl.id_client = cc.id_client " +
                "LEFT JOIN `_tf_plan` AS tp ON tp.id_tplan = cc.id_tplan " +
                $"WHERE cc.name like '%{name}%' " +
                "AND cc.`_working` = 1 " +
                "GROUP BY cc.id_client, cc.name, cc.telefon_T, cc.t_email";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCOD>(query);

    return result;
  }

  async Task<IEnumerable<ClientCOD>> IClientRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT cc.id_client AS \"Id\", cc.name AS \"Name\", cc.telefon_T AS \"Contact\", cc.t_email AS \"Email\", " +
                "tp.name_tplan AS \"TPlan\", " +
                "GROUP_CONCAT(CAST(vl.id_vlan AS INT) ORDER BY vl.id_vlan) AS \"VLANTags\" " +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS vl ON vl.id_client = cc.id_client " +
                "LEFT JOIN `_tf_plan` AS tp ON tp.id_tplan = cc.id_tplan " +
                "WHERE cc.`_working` = 1 " +
                "GROUP BY cc.id_client, cc.name, cc.telefon_T, cc.t_email";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCOD>(query);

    return result;
  }

  async Task<string?> IClientRepository.GetClientNameByVlanTagAsync(int tag,
                                                                    CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.name AS \"Name\"" +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS vl ON vl.id_client = cc.id_client " +
                "WHERE vl.id_vlan = @Tag " +
                "AND cc.`_working` = 1";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.ExecuteScalarAsync<string>(query, new { Tag = tag });
    return result;
  }

  async Task<ClientCOD> IClientRepository.GetByNameAsync(string clientName,
                                                         CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.id_client AS \"Id\", " +
                "cc.name AS \"Name\", " +
                "cc.telefon_T AS \"Contact\", " +
                "cc.t_email AS \"Email\", " +
                "tp.name_tplan AS \"TPlan\", " +
                "GROUP_CONCAT(CAST(vl.id_vlan AS INT) ORDER BY vl.id_vlan) AS \"VLANTags\" " +
                "FROM client_cod AS cc " +
                "LEFT JOIN `_spr_vlan` AS vl ON vl.id_client = cc.id_client " +
                "LEFT JOIN `_tf_plan` AS tp ON tp.id_tplan = cc.id_tplan " +
                "WHERE cc.name = @ClientName " +
                "AND cc.`_working` = 1 " +
                "GROUP BY cc.id_client, cc.name, cc.telefon_T, cc.t_email";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryFirstAsync<ClientCOD>(query, new { ClientName = clientName });

    return result;
  }
}
