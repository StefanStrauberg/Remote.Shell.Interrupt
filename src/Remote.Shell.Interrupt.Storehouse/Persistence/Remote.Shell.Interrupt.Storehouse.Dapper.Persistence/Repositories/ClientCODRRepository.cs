namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ClientCODRRepository(MySQLDapperContext mySQLDapperContext) : IClientCODRRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<ClientCODR>> IClientCODRRepository.GetAllAsync(CancellationToken cancellationToken)
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
                "cc.id_cod as \"Id_COD\", " +
                "cc.id_tplan as \"Id_TfPlan\", " +
                "cc.history as \"History\", " +
                "cc.ad as \"AntiDDOS\" " +
                "FROM client_cod AS cc";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<ClientCODR>(query);

    return result;
  }
}
