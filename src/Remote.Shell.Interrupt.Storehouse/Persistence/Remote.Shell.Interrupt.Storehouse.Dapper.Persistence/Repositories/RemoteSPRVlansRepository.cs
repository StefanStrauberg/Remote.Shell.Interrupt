namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class RemoteSPRVlansRepository(MySQLDapperContext mySQLDapperContext) 
  : IRemoteSPRVlansRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<RemoteSPRVlan>> IRemoteSPRVlansRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "spr.id_vlan as \"IdVlan\", " +
                "spr.id_client as \"IdClient\", " +
                "spr.use_client as \"UseClient\", " +
                "spr.use_cod as \"UseCOD\" " +
                "FROM `_spr_vlan` as spr";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<RemoteSPRVlan>(query);
  }
}
