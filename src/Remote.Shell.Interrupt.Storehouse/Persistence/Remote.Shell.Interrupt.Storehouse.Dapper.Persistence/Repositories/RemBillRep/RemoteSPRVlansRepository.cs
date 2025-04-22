namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.RemBillRep;

internal class RemoteSPRVlansRepository(MySQLDapperContext context) 
  : IRemoteSPRVlansRepository
{
  async Task<IEnumerable<RemoteSPRVlan>> IRemoteGenericRepository<RemoteSPRVlan>.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "spr.id_vlan as \"IdVlan\", " +
                "spr.id_client as \"IdClient\", " +
                "spr.use_client as \"UseClient\", " +
                "spr.use_cod as \"UseCOD\" " +
                "FROM `_spr_vlan` as spr";
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<RemoteSPRVlan>(query);
  }
}
