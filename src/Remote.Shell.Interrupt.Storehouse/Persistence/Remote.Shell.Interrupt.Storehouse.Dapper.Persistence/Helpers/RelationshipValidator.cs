namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class RelationshipValidator
{
  public static void ValidateManyToManyRelationship(ManyToManyRelationship rel)
  {
    if (rel.JoinEntity == null)
      throw new InvalidOperationException($"JoinEntity required for {rel.NavigationProperty}");
    
    if (string.IsNullOrEmpty(rel.PrincipalForeignKey) || string.IsNullOrEmpty(rel.DependentForeignKey))
      throw new InvalidOperationException($"Both foreign keys required for {rel.NavigationProperty}");
  }
}
