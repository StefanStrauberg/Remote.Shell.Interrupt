namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class OneQueryRepository<T>(PostgreSQLDapperContext context,
                                     IAppLogger<OneQueryRepository<T>> logger)
  : IOneQueryRepository<T> where T : BaseEntity
{
  async Task<T> IOneQueryRepository<T>.GetOneShortAsync(ISpecification<T> specification,
                                                        CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<T>(specification);

    var sql = queryBuilder.Build();

    logger.LogInformation(sql);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QuerySingleAsync<T>(sql);
  }
}
