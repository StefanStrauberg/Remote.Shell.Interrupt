namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientRepository(MySQLDapperContext mySQLDapperContext) : IClientCODRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCodR>> IClientCODRepository.GetAllByNameAsync(string name,
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
    var result = await connection.QueryAsync<ClientCodR>(query);

    return result;
  }

  async Task<IEnumerable<ClientCodR>> IClientCODRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "cc.id_client as \"Id\", " +
                "cc.name as \"Name\", " +
                "cc.contact_C as \"ContactC\", " +
                "cc.telefon_C as \"TelephoneC\", " +
                "cc.contact_T as \"ContactT\", " +
                "cc.telefon_T as \"TelephoneT\", " +
                "cc.c_email as \"EmailC\", " +
                "cc.`_working` as \"Working\", " +
                "cc.t_email as \"EmailT\", " +
                "cc.id_cod as \"IdCOD\", " +
                "cc.id_tplan as \"IdTfPlan\", " +
                "cc.history as \"History\", " +
                "cc.ad as \"AntiDDOS\", " +
                "c.ID_cod AS \"Id\", " +
                "c.name_cod AS \"NameCOD\", " +
                "c.telephone AS \"Telephone\", " +
                "c.e_mail AS \"Email1\", " +
                "c.e_mail2 AS \"Email2\", " +
                "c.contact AS \"Contact\", " +
                "c.description AS \"Description\", " +
                "c.region AS \"Region\", " +
                "tp.id_tplan AS \"Id\", " +
                "tp.name_tplan AS \"NameTfPlan\", " +
                "tp.descr_tplan AS \"DescTfPlan\" " +
                "FROM client_cod as cc " +
                "LEFT JOIN `_cods`AS c ON c.ID_cod = cc.id_cod " +
                "LEFT JOIN `_tf_plan` AS tp ON tp.id_tplan = cc.id_tplan";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var ccDictionary = new Dictionary<int, ClientCodR>();
    var cDicotionary = new Dictionary<int, CODR>();
    var tpDictionary = new Dictionary<int, TfPlanR>();

    await connection.QueryAsync(
        query,
        (Func<ClientCodR, CODR, TfPlanR, ClientCodR>)((cc, c, tp) =>
        {
          if (!ccDictionary.TryGetValue(cc.Id, out var clientCodR))
          {
            clientCodR = cc;
            ccDictionary.Add(clientCodR.Id, clientCodR);
          }

          if (c is not null && !cDicotionary.TryGetValue(c.Id, out var CODR))
          {
            CODR = c;
            clientCodR.COD = c;
            cDicotionary.Add(CODR.Id, CODR);
          }

          if (tp is not null && !tpDictionary.TryGetValue(tp.Id, out var TfPlanR))
          {
            TfPlanR = tp;
            clientCodR.TfPlan = tp;
            tpDictionary.Add(TfPlanR.Id, TfPlanR);
          }

          return clientCodR;
        }),
        splitOn: "Id, Id, Id");

    return ccDictionary.Values.ToList();
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
