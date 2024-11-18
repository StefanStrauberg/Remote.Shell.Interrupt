namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class BusinessRuleRepository(DapperContext context) : GenericRepository<BusinessRule>(context), IBusinessRuleRepository
{
  public async Task<BusinessRule> GetBusinessRulesTreeAsync(CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"WITH RECURSIVE RecursiveBusinessRules AS (" +
                "SELECT " +
                "\"Id\", " +
                "\"Name\", " +
                "\"Vendor\", " +
                "\"IsRoot\", " +
                "\"ParentId\", " +
                "\"AssignmentId\" " +
                "FROM \"BusinessRules\" " +
                "WHERE \"ParentId\" IS NULL " +
                "UNION ALL " +
                "SELECT " +
                "child.\"Id\", " +
                "child.\"Name\", " +
                "child.\"Vendor\", " +
                "child.\"IsRoot\", " +
                "child.\"ParentId\", " +
                "child.\"AssignmentId\" " +
                "FROM \"BusinessRules\" AS child " +
                "INNER JOIN RecursiveBusinessRules rbr ON child.\"ParentId\" = rbr.\"Id\" " +
                ") " +
                "SELECT * " +
                "FROM RecursiveBusinessRules;";
    using var connection = _context.CreateConnection();
    var businessRulesDictionary = new Dictionary<Guid, BusinessRule>();
    var businessRules = await connection.QueryAsync<BusinessRule>(query);
    // Организуем категории в иерархическую структуру
    foreach (var businessRule in businessRules)
    {
      if (!businessRulesDictionary.TryGetValue(businessRule.Id, out var businessRuleEntry))
      {
        businessRuleEntry = businessRule;
        businessRuleEntry.Children = [];
        businessRulesDictionary[businessRuleEntry.Id] = businessRuleEntry;
      }

      if (businessRule.ParentId.HasValue)
      {
        // Если у категории есть родитель, добавляем ее в список детей родителя
        if (businessRulesDictionary.TryGetValue(businessRule.ParentId.Value, out var parentEntry))
        {
          parentEntry.Children.Add(businessRuleEntry);
        }
      }
    }
    return businessRulesDictionary.Values.First(c => c.ParentId == null);
  }

  public async Task<BusinessRule> GetBusinessRulesNodeByIdAsync(Guid id,
                                                                CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var query = $"WITH RECURSIVE RecursiveBusinessRules AS (" +
                "SELECT " +
                "\"Id\", " +
                "\"Name\", " +
                "\"Vendor\", " +
                "\"IsRoot\", " +
                "\"ParentId\", " +
                "\"AssignmentId\" " +
                "FROM \"BusinessRules\" " +
                "WHERE \"Id\"=@Id " +
                "UNION ALL " +
                "SELECT " +
                "child.\"Id\", " +
                "child.\"Name\", " +
                "child.\"Vendor\", " +
                "child.\"IsRoot\", " +
                "child.\"ParentId\", " +
                "child.\"AssignmentId\" " +
                "FROM \"BusinessRules\" AS child " +
                "INNER JOIN RecursiveBusinessRules rbr ON child.\"ParentId\" = rbr.\"Id\" " +
                ") " +
                "SELECT * " +
                "FROM RecursiveBusinessRules;";
    using var connection = _context.CreateConnection();
    var businessRulesDictionary = new Dictionary<Guid, BusinessRule>();
    var businessRules = await connection.QueryAsync<BusinessRule>(query, new { Id = id });
    // Организуем категории в иерархическую структуру
    foreach (var businessRule in businessRules)
    {
      if (!businessRulesDictionary.TryGetValue(businessRule.Id, out var businessRuleEntry))
      {
        businessRuleEntry = businessRule;
        businessRuleEntry.Children = [];
        businessRulesDictionary[businessRuleEntry.Id] = businessRuleEntry;
      }

      if (businessRule.ParentId.HasValue)
      {
        // Если у категории есть родитель, добавляем ее в список детей родителя
        if (businessRulesDictionary.TryGetValue(businessRule.ParentId.Value, out var parentEntry))
        {
          parentEntry.Children.Add(businessRuleEntry);
        }
      }
    }
    return businessRulesDictionary.Values.First(c => c.Id == id);
  }
}
