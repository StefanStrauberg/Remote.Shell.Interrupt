namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlanRsRepository(MySQLDapperContext mySQLDapperContext) : ISPRVlanRsRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<SPRVlanR>> ISPRVlanRsRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "spr.id_vlan as \"IdVlan\", " +
                "spr.id_client as \"IdClient\", " +
                "spr.use_client as \"UseClient\", " +
                "spr.use_cod as \"UseCOD\" " +
                "FROM `_spr_vlan` as spr";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<SPRVlanR>(query);

    return result;
  }
}
