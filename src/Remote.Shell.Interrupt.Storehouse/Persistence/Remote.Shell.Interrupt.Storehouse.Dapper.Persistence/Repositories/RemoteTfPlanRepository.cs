namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class RemoteTfPlanRepository(MySQLDapperContext mySQLDapperContext) : IRemoteTfPlanRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<RemoteTfPlan>> IRemoteTfPlanRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "tp.id_tplan AS \"IdTfPlan\", " +
                "tp.name_tplan AS \"NameTfPlan\", " +
                "tp.descr_tplan AS \"DescTfPlan\" " +
                "FROM `_tf_plan` as tp";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<RemoteTfPlan>(query);

    return result;
  }
}
