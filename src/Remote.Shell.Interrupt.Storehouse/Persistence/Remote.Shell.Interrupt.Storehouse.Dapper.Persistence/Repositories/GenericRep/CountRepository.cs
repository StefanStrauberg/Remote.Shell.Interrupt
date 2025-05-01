namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(PostgreSQLDapperContext context)
  : ICountRepository<T> where T : BaseEntity
{
  async Task<int> ICountRepository<T>.GetCountAsync(ISpecification<T> specification,
                                                    CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<T>(specification);

    var sql = queryBuilder.BuildCount();

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.ExecuteScalarAsync<int>(sql);
  }
}
