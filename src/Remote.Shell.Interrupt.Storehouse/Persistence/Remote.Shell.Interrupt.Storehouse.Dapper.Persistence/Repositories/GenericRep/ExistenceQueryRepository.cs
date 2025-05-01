namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ExistenceQueryRepository<T>(PostgreSQLDapperContext context)
  : IExistenceQueryRepository<T> where T : BaseEntity
{
  async Task<bool> IExistenceQueryRepository<T>.AnyByQueryAsync(ISpecification<T> specification,
                                                                CancellationToken cancellationToken)
  {
    var queryBuilder = new SqlQueryBuilder<T>(specification);

    var sql = queryBuilder.BuildCount();

    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.ExecuteScalarAsync<int>(sql) > 0;
  }
}
