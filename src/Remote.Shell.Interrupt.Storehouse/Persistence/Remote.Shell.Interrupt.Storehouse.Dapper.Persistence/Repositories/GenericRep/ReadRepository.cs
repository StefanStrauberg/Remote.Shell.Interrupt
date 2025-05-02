namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReadRepository<T>(PostgreSQLDapperContext context,
                                 IAppLogger<ReadRepository<T>> logger)
  : IReadRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IReadRepository<T>.GetAllAsync(CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<T>();

    var sql = queryBuilder.Build();

    logger.LogInformation(sql);

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<T>(sql);
  }
}
