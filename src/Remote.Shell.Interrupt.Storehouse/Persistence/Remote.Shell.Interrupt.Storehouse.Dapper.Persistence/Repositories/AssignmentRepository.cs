namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class AssignmentRepository(DapperContext context) : GenericRepository<Assignment>(context), IAssignmentRepository
{
  public async Task<bool> AnyWithTheSameNameAndDifferentIdAsync(Guid id,
                                                                string name,
                                                                CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $@"SELECT EXISTS ( " +
                $"SELECT 1 " +
                $"FROM \"{tableName}\" " +
                "WHERE \"Name\"=@Name AND \"Id\"!=@Id)";
    using var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query, new { Name = name, Id = id });
    return exists;
  }

  public async Task<bool> AnyWithTheSameNameAsync(string name, CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $@"SELECT EXISTS ( " +
                $"SELECT 1 " +
                $"FROM \"{tableName}\" " +
                "WHERE \"Name\"=@Name)";
    using var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query, new { Name = name });
    return exists;
  }
}
