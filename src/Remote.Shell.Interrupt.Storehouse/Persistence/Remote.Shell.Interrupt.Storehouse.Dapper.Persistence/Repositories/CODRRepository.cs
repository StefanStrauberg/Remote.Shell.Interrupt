namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class CODRRepository(MySQLDapperContext mySQLDapperContext) : ICODRRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<CODR>> ICODRRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "c.ID_cod AS \"IdCOD\"," +
                "c.name_cod AS \"NameCOD\", " +
                "c.telephone AS \"Telephone\", " +
                "c.e_mail AS \"Email1\", " +
                "c.e_mail2 AS \"Email2\", " +
                "c.contact AS \"Contact\", " +
                "c.description AS \"Description\", " +
                "c.region AS \"Region\" " +
                "FROM `_cods` as c";
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);

    var result = await connection.QueryAsync<CODR>(query);

    return result;
  }
}

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