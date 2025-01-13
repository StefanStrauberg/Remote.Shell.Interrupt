namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TfPlanRRepository(MySQLDapperContext mySQLDapperContext) : ITfPlanRRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<TfPlanR>> ITfPlanRRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "tp.id_tplan AS \"IdTfPlan\", " +
                "tp.name_tplan AS \"NameTfPlan\", " +
                "tp.descr_tplan AS \"DescTfPlan\" " +
                "FROM `_tf_plan` as tp";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<TfPlanR>(query);

    return result;
  }
}
