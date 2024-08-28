namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.BusinessRuleDTOs;

public class CreateBusinessRuleDTO
{
  public string Name { get; set; } = string.Empty;
  public string? Condition { get; set; } // TODO Convert from string to Expression<Func<BusinessRule, bool>> Condition
  public Guid? ParentId { get; set; } // Use ParentId instead of Branch
  public Guid? AssignmentId { get; set; }
}
