namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class DbSet<TEntity>(ModelBuilder modelBuilder, DbContext context) 
  where TEntity : class
{
  readonly ModelBuilder _modelBuilder = modelBuilder;
  readonly DbContext _context = context;
  readonly JsonSerializerOptions _jsonSerializerOptions = new()
  {
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };
  readonly List<string> _includes = [];
  string _whereClause = string.Empty;
  object _whereParameters = null!;
  int? _skip;
  int? _take;
  SQLQueryBuilder<TEntity> _sqlQueryBuilder = null!;
  string _customSelect = "*";
  string _fromClause = "";

  public DbSet<TEntity> Select(Expression<Func<TEntity, object>> selector)
  {
      var visitor = new SelectExpressionVisitor();
      visitor.Visit(selector);
      _customSelect = visitor.SelectedColumns;
      return this;
  }

  public DbSet<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty)
  {
    var member = ((MemberExpression)navigationProperty.Body).Member.Name;
    _includes.Add(member);
    return this;
  }

  public DbSet<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
  {
    var visitor = new SqlExpressionVisitor();
    visitor.Visit(predicate);
    _whereClause = visitor.WhereClause;
    _whereParameters = visitor.Parameters;
    return this;
  }

  public DbSet<TEntity> FromSqlRaw(string sql)
  {   
    _fromClause = $"FROM ({sql}) AS custom_query";
    return this;
  }

  public DbSet<TEntity> Skip(int skip)
  {
    _skip = skip;
    return this;
  }

  public DbSet<TEntity> Take(int take)
  {
    _take = take;
    return this;
  }

  public async Task<IEnumerable<TEntity>> ToListAsync(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    return await connection.QueryAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<TEntity> FirstAsync(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    return await connection.QueryFirstAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<TEntity?> FirstOrDefaultAsync(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<int> CountAsync(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take)
    {
      CustomSelect = $"COUNT({_customSelect})"
    };
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    return await connection.ExecuteScalarAsync<int>(sql, _whereParameters);
  }

  public async Task<bool> AnyAsync(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take)
    {
      CustomSelect = $"COUNT({_customSelect})"
    };
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    return await connection.ExecuteScalarAsync<int>(sql, _whereParameters) > 0;
  }

  public void Delete(IDbConnection connection)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    connection.Execute(sql, _whereParameters);
  }

  public Guid Insert(IDbConnection connection, TEntity entity)
  {
    _sqlQueryBuilder = new(_modelBuilder, null!, null!, null!, null!);
    var sql = _sqlQueryBuilder.BuildQuery();

    if (_context.EnableSqlLogging)
      LogQuery(sql, null!);

    return connection.ExecuteScalar<Guid>(sql, entity);
  }

  public void Update(IDbConnection connection, TEntity entity)
  {
    _sqlQueryBuilder = new(_modelBuilder, null!, null!, null!, null!);
    var sql = _sqlQueryBuilder.BuildQuery();

    if (_context.EnableSqlLogging)
      LogQuery(sql, null!);

    connection.Execute(sql, entity);
  }

  public async Task<IEnumerable<TEntity>> ExecuteRawQueryAsync(IDbConnection connection)
  {
    var sql = $"SELECT {_customSelect} {_fromClause}";
      
    if (!string.IsNullOrEmpty(_whereClause))
      sql += $" WHERE {_whereClause}";
      
    if (_take.HasValue)
      sql += $" LIMIT {_take}";

    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    return await connection.QueryAsync<TEntity>(sql, _whereParameters ?? 0);
  }

  void LogQuery(string sql, object parameters)
  {
    if (_context.Logger == null) return;

    var className = $"{typeof(DbSet<TEntity>).Name}<{typeof(TEntity).Name}>";

    var logMessage = $"""
        SQL Query:
        {sql}
        Parameters: {SerializeParameters(parameters)}
        """;

    _context.Logger.LogInformation(className, logMessage);
  }

  string SerializeParameters(object parameters)
  {
    if (parameters == null) 
      return "{}";
    
    return JsonSerializer.Serialize(parameters, _jsonSerializerOptions);
  }
}
