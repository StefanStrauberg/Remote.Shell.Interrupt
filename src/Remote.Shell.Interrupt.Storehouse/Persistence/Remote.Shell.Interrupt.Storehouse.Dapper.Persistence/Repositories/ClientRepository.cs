namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository(MySQLDapperContext mySQLDapperContext) : IClientCODRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCod>> IClientCODRepository.GetAllByNameAsync(string name,
                                                                         CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.id_client as \"IdClient\", " +
                "cc.name as \"Name\", " +
                "cc.contact_C as \"ContactC\", " +
                "cc.telefon_C as \"TelephoneC\", " +
                "cc.contact_T as \"ContactT\", " +
                "cc.telefon_T as \"TelephoneT\", " +
                "cc.c_email as \"EmailC\", " +
                "cc.`_working` as \"Working\", " +
                "cc.t_email as \"EmailT\", " +
                "cc.id_cod as \"IdCOD\", " +
                "cc.id_tplan as \"IdTPlan\", " +
                "cc.history as \"History\", " +
                "cc.ad as \"AntiDDOS\" " +
                "FROM client_cod as cc " +
                $"WHERE cc.name like '%{name}%' " +
                "AND cc.`_working` = 1 ";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCod>(query);

    return result;
  }

  async Task<IEnumerable<ClientCod>> IClientCODRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.id_client as \"IdClient\", " +
                "cc.name as \"Name\", " +
                "cc.contact_C as \"ContactC\", " +
                "cc.telefon_C as \"TelephoneC\", " +
                "cc.contact_T as \"ContactT\", " +
                "cc.telefon_T as \"TelephoneT\", " +
                "cc.c_email as \"EmailC\", " +
                "cc.`_working` as \"Working\", " +
                "cc.t_email as \"EmailT\", " +
                "cc.id_cod as \"IdCOD\", " +
                "cc.id_tplan as \"IdTPlan\", " +
                "cc.history as \"History\", " +
                "cc.ad as \"AntiDDOS\" " +
                "FROM client_cod as cc";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    var result = await connection.QueryAsync<ClientCod>(query);

    return result;
  }

  async Task<string?> IClientCODRepository.GetClientNameByVlanTagAsync(int tag,
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
}
