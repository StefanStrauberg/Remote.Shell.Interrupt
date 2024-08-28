namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.BusinessRuleDTOs;

public class UpdateBusinessRuleDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Condition { get; set; } = string.Empty;
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; }
}
