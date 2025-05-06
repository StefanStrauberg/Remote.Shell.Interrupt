namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class Relationship
{
  public Type PrincipalEntity { get; set; } = null!;
  public Type DependentEntity { get; set; } = null!;
  public string NavigationProperty { get; set; } = string.Empty;
  public bool IsRequired { get; set; } = false;
  public RelationshipType RelationshipType { get; set; }
}

internal class OneToManyRelationship : Relationship
{
  public string ForeignKey { get; set; } = string.Empty;
}

internal class OneToOneRelationship : Relationship
{
  public string ForeignKey { get; set; } = string.Empty;
}

internal class ManyToManyRelationship : Relationship
{
  public Type JoinEntity { get; set; } = null!;
  public string LeftForeignKey { get; set; } = string.Empty;
  public string RightForeignKey { get; set; } = string.Empty;
}