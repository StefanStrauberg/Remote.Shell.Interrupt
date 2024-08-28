namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.BusinessRuleDTOs;

public class CreateBusinessRuleDTO
{
  public string Name { get; set; } = string.Empty;
  public string? Condition { get; set; } // TODO Convert from string to Expression<Func<BusinessRule, bool>> Condition
  public List<int> Branch { get; set; } = [];
  public Guid? AssignmentId { get; set; }
}
