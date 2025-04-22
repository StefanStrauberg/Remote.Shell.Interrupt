namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.RemBillRep;

internal class RemoteTfPlanRepository(MySQLDapperContext context) 
  : IRemoteTfPlanRepository
{
  async Task<IEnumerable<RemoteTfPlan>> IRemoteGenericRepository<RemoteTfPlan>.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "tp.id_tplan AS \"IdTfPlan\", " +
                "tp.name_tplan AS \"NameTfPlan\", " +
                "tp.descr_tplan AS \"DescTfPlan\" " +
                "FROM `_tf_plan` as tp";
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<RemoteTfPlan>(query);
  }
}
