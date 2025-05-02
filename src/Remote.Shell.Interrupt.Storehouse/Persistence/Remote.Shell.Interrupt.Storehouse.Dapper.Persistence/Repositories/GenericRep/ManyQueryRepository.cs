namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ManyQueryRepository<T>(PostgreSQLDapperContext context,
                                      IAppLogger<ManyQueryRepository<T>> logger)
  : IManyQueryRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IManyQueryRepository<T>.GetManyShortAsync(ISpecification<T> specification,
                                                                       CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<T>(specification);

    var sql = queryBuilder.Build();

    logger.LogInformation(sql);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<T>(sql);
  }
}
