namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class EntityConfiguration(Type entityType)
{
  public Type EntityType { get; } = entityType;
  public List<Relationship> Relationships { get; } = [];
  public string TableName { get; set; } = entityType.Name;
  public string PrimaryKey { get; set; } = "Id";
  public List<string> Columns { get; } = [.. entityType.GetProperties().Select(p => p.Name)];
}
