namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class DbSet<TEntity>(ModelBuilder modelBuilder, DbContext context) 
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
    // Если есть хотя бы одно включение, значит алиасы используются в JOIN, иначе — нет
    var alias = _includes.Count > 0 ? _modelBuilder.Configurations[typeof(TEntity)].TableName.ToLower()
                                    : null;
    var visitor = new SqlExpressionVisitor(alias);
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

  public async Task<IEnumerable<TEntity>> ToListAsync()
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = await _context.GetConnectionAsync();
    
    return await connection.QueryAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<TEntity> FirstAsync()
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);
    
    var connection = await _context.GetConnectionAsync();
    
    return await connection.QueryFirstAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<TEntity?> FirstOrDefaultAsync()
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = await _context.GetConnectionAsync();
    
    return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, _whereParameters);
  }

  public async Task<int> CountAsync()
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take)
    {
      CustomSelect = $"COUNT({_customSelect})"
    };

    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = await _context.GetConnectionAsync();

    return await connection.ExecuteScalarAsync<int>(sql, _whereParameters);
  }

  public async Task<bool> AnyAsync()
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take)
    {
      CustomSelect = $"COUNT({_customSelect})"
    };

    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = await _context.GetConnectionAsync();
    
    return await connection.ExecuteScalarAsync<int>(sql, _whereParameters) > 0;
  }

  public void Delete(TEntity entity)
  {
    _sqlQueryBuilder = new(_modelBuilder, _includes, _whereClause, _skip, _take);
    var sql = _sqlQueryBuilder.BuildQuery();
    
    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = _context.GetConnection();
    
    connection.Execute(sql, _whereParameters);
  }

  public Guid Insert(TEntity entity)
  {
    _sqlQueryBuilder = new(_modelBuilder, null!, null!, null!, null!);
    var sql = _sqlQueryBuilder.BuildQuery();

    if (_context.EnableSqlLogging)
      LogQuery(sql, null!);

    var connection = _context.GetConnection();

    return connection.ExecuteScalar<Guid>(sql, entity);
  }

  public void Update(TEntity entity)
  {
    _sqlQueryBuilder = new(_modelBuilder, null!, null!, null!, null!);
    var sql = _sqlQueryBuilder.BuildQuery();

    if (_context.EnableSqlLogging)
      LogQuery(sql, null!);

    var connection = _context.GetConnection();

    connection.Execute(sql, entity);
  }

  public async Task<IEnumerable<TEntity>> ExecuteRawQueryAsync()
  {
    var sql = $"SELECT {_customSelect} {_fromClause}";
      
    if (!string.IsNullOrEmpty(_whereClause))
      sql += $" WHERE {_whereClause}";
      
    if (_take.HasValue)
      sql += $" LIMIT {_take}";

    if (_context.EnableSqlLogging)
      LogQuery(sql, _whereParameters);

    var connection = await _context.GetConnectionAsync();

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

  internal static DbSet<TEntity> Create(ModelBuilder modelBuilder, DbContext dbContext)
    => new(modelBuilder, dbContext);
}
