namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class AssignmentRepository(DapperContext context) : GenericRepository<Assignment>(context), IAssignmentRepository
{
  async Task<bool> IAssignmentRepository.AnyWithTheSameNameAndDifferentIdAsync(Guid id,
                                                                               string name,
                                                                               CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT COUNT(1) FROM \"{tableName}\" WHERE \"Name\"=@Name AND \"Id\"!=@Id";
    var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query, new { Name = name, Id = id });
    return exists;
  }

  async Task<bool> IAssignmentRepository.AnyWithTheSameNameAsync(string name,
                                                                 CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    var query = $"SELECT COUNT(1) FROM \"{tableName}\" WHERE \"Name\"=@Name";
    var connection = _context.CreateConnection();
    var exists = await connection.ExecuteScalarAsync<bool>(query, new { Name = name });
    return exists;
  }
}
