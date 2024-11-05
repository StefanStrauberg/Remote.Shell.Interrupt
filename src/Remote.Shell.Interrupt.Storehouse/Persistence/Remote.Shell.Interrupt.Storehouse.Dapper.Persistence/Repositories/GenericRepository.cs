namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

public class GenericRepository<T>(DapperContext context)
  : IGenericRepository<T> where T : BaseEntity
{
  private readonly DapperContext _context = context;

  public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public void DeleteMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }

  public void DeleteOne(T entity)
  {
    throw new NotImplementedException();
  }

  public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
  {
    var query = $"SELECT * FROM {typeof(T).Name}s WHERE {SqlHelper<T>.GetWhereClause(predicate)} LIMIT 1";

    using var connection = _context.CreateConnection();

    var result = await connection.QueryFirstOrDefaultAsync<T>(query, cancellationToken);

    return result ?? throw new Exception($"During sql \"{query}\" execution was receive null value.");
  }

  public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT * FROM {typeof(T).Name}s";

    using var connection = _context.CreateConnection();

    var result = await connection.QueryAsync<T>(query, cancellationToken);

    return result.ToList();
  }

  public void InsertMany(IEnumerable<T> entities)
  {
    throw new NotImplementedException();
  }

  public void InsertOne(T entity)
  {
    throw new NotImplementedException();
  }

  public void ReplaceOne(T entity)
  {
    throw new NotImplementedException();
  }
}
