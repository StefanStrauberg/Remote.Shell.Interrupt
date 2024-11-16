namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class BusinessRuleRepository(DapperContext context) : GenericRepository<BusinessRule>(context), IBusinessRuleRepository
{
  public async Task<IEnumerable<BusinessRule>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"SELECT " +
                "parent.\"Id\" AS Id, " +
                "parent.\"Name\" AS Name, " +
                "parent.\"Vendor\" AS Vendor, " +
                "parent.\"IsRoot\" AS IsRoot, " +
                "parent.\"ParentId\" AS ParentId, " +
                "parent.\"AssignmentId\" AS AssignmentId, " +
                "child.\"Id\" AS ChildId, " +
                "child.\"Name\" AS ChildName, " +
                "child.\"Vendor\" AS ChildVendor, " +
                "child.\"IsRoot\" AS ChildIsRoot, " +
                "child.\"ParentId\" AS ChildParentId, " +
                "child.\"AssignmentId\" AS ChildAssignmentId " +
                $"FROM \"{tableName}\" AS parent " +
                $"LEFT JOIN \"{tableName}\" AS child " +
                "ON child.\"ParentId\" = parent.\"Id\"";
    using var connection = _context.CreateConnection();
    var businessRulesDictionary = new Dictionary<Guid, BusinessRule>();
    var businessRules = await connection.QueryAsync<BusinessRule, BusinessRule, BusinessRule>(
        query, (parent, child) =>
        {
          parent.Children = [child];
          return parent;
        },
        splitOn: "ChildId" // указывает Dapper, где начинается второй объект
    );
    return businessRules.ToList();
  }
}
