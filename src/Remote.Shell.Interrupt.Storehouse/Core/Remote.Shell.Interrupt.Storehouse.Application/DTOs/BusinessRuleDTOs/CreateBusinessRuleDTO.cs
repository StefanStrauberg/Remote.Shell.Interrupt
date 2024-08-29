namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.BusinessRuleDTOs;

public class CreateBusinessRuleDTO
{
  public string Name { get; set; } = string.Empty;
  public string? Condition { get; set; }
  public Guid? ParentId { get; set; }
  public Guid? AssignmentId { get; set; }
}
