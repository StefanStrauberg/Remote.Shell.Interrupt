namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.BusinessRuleDTOs;

public class BusinessRuleDTO
{
  public Guid Id { get; private set; }
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; } // Represent what to do
}
