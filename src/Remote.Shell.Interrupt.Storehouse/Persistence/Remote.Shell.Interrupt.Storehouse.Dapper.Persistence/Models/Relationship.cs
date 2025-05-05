namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class Relationship
{
  public Type PrincipalEntity { get; set; } = null!;
  public Type DependentEntity { get; set; } = null!;
  public string ForeignKey { get; set; } = string.Empty;
  public string NavigationProperty { get; set; } = string.Empty;
  public bool IsManyToMany { get; set; }
  public Type JoinEntity { get; set; } = null!;
  public bool IsRequired { get; set; } = true;
  public bool IsNullable { get; set; }
  public RelationshipType RelationshipType { get; set; }
}
