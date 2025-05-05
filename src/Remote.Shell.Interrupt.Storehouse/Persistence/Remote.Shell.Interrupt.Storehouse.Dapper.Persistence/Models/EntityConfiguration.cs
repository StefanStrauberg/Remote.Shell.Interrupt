namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class EntityConfiguration(Type entityType)
{
  private string _primaryKey = "Id";

  public Type EntityType { get; } = entityType ?? throw new ArgumentNullException(nameof(entityType));
  public List<Relationship> Relationships { get; } = [];
  public string TableName { get; set; } = GetTableName.Handle(entityType.Name);
  public string PrimaryKey
  {
    get => _primaryKey;
    set
    {
      if (!Columns.Contains(value))
        throw new ArgumentException($"Property '{value}' not found in {EntityType.Name}");
      _primaryKey = value;
    }
  }
  public List<string> Columns { get; } = [.. entityType.GetProperties().Select(p => p.Name)];

  public void Validate()
  {
    foreach (var rel in Relationships)
    {
      if (!Columns.Contains(rel.NavigationProperty))
        throw new InvalidOperationException(
          $"Navigation property '{rel.NavigationProperty}' not found in {EntityType.Name}"
        );
    }

    if (!Columns.Contains(PrimaryKey))
      throw new InvalidOperationException(
        $"Primary key '{PrimaryKey}' is invalid for {EntityType.Name}"
      );
  }
}
