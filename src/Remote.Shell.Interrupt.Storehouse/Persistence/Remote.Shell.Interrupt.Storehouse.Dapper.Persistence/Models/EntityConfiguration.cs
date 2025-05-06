namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class EntityConfiguration(Type entityType)
{
  string _primaryKey = nameof(BaseEntity.Id);

  public Type EntityType { get; } = entityType ?? throw new ArgumentNullException(nameof(entityType));
  public List<Relationship> Relationships { get; } = [];
  public string TableName { get; set; } = GetTableName.Handle(entityType.Name);
  public List<string> Properties { get; } = [.. entityType.GetProperties().Select(p => p.Name)];
  public string PrimaryKey
  {
    get => _primaryKey;
    set => ValidateAndSetPrimaryKey(value);
  }

  public void Validate()
  {
    ValidatePrimaryKey();
    ValidateRelationships();
  }

  void ValidateAndSetPrimaryKey(string value)
  {
    if (!Properties.Contains(value))
        throw new ArgumentException($"Property '{value}' not found in {EntityType.Name}");
    _primaryKey = value;
  }

  void ValidatePrimaryKey()
  {
    if (!Properties.Contains(PrimaryKey))
        throw new InvalidOperationException($"Invalid primary key '{PrimaryKey}' for {EntityType.Name}");
  }

  void ValidateRelationships()
  {
    foreach (var rel in Relationships)
    {
      ValidateNavigationProperty(rel);
      ValidateRelationshipSpecifics(rel);
    }
  }

  void ValidateNavigationProperty(Relationship rel)
  {
    if (!Properties.Contains(rel.NavigationProperty))
      throw new InvalidOperationException($"Navigation property '{rel.NavigationProperty}' not found in {EntityType.Name}");
  }

  static void ValidateRelationshipSpecifics(Relationship rel)
  {
    switch (rel)
    {
      case OneToManyRelationship oneToMany:
        if (string.IsNullOrEmpty(oneToMany.ForeignKey))
            throw new InvalidOperationException($"ForeignKey required for {rel.NavigationProperty}");
        break;
      
      case ManyToManyRelationship manyToMany:
        ValidateManyToManyRelationship(manyToMany);
        break;
    }
  }

  static void ValidateManyToManyRelationship(ManyToManyRelationship rel)
  {
    if (rel.JoinEntity == null)
      throw new InvalidOperationException($"JoinEntity required for {rel.NavigationProperty}");
    
    if (string.IsNullOrEmpty(rel.LeftForeignKey) || string.IsNullOrEmpty(rel.RightForeignKey))
      throw new InvalidOperationException($"Both foreign keys required for {rel.NavigationProperty}");
  }
}
