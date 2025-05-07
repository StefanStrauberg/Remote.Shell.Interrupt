namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

/// <summary>
/// Represents an entity configuration that defines metadata, relationships, and validation rules.
/// </summary>
internal class EntityConfiguration(Type entityType)
{
  /// <summary>
  /// Stores the primary key name for the entity.
  /// </summary>
  string _primaryKey = nameof(BaseEntity.Id);

  /// <summary>
  /// Stores the entity type associated with this configuration.
  /// </summary>
  readonly Type _entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

  /// <summary>
  /// Gets the entity type.
  /// </summary>
  public Type EntityType => _entityType;

  /// <summary>
  /// Gets the collection of relationships defined for the entity.
  /// </summary>
  public List<Relationship> Relationships { get; } = [];

  /// <summary>
  /// Gets or sets the table name mapped to this entity.
  /// </summary>
  public string TableName { get; set; } = GetTableName.Handle(entityType.Name);

  /// <summary>
  /// Gets the list of property names belonging to the entity.
  /// </summary>
  public List<string> Properties { get; } = [.. entityType.GetProperties().Select(p => p.Name)];

  /// <summary>
  /// Gets or sets the primary key for the entity, ensuring its validity.
  /// </summary>
  public string PrimaryKey
  {
    get => _primaryKey;
    set => ValidateAndSetPrimaryKey(value);
  }

  /// <summary>
  /// Validates and sets the primary key if it exists in the entity properties.
  /// </summary>
  /// <param name="value">The primary key name to validate and set.</param>
  /// <exception cref="ArgumentException">Thrown if the specified key is not found in the entity properties.</exception>
  void ValidateAndSetPrimaryKey(string value)
  {
    if (!Properties.Contains(value))
        throw new ArgumentException($"Property '{value}' not found in {EntityType.Name}");
    _primaryKey = value;
  }

  /// <summary>
  /// Validates the entity configuration by ensuring primary key and relationships are correctly defined.
  /// </summary>
  public void Validate()
  {
    EntityTypeValidatior.ValidatePrimaryKey(this);
    EntityTypeValidatior.ValidateRelationships(this);
  }
}
