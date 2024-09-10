namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public bool IsRoot { get; set; }

  public Guid? ParentId { get; set; }
  public BusinessRule? Parent { get; set; }

  public Guid AssignmentId { get; set; }
  public Assignment Assignment { get; set; } = null!;

  public List<BusinessRule> Children { get; set; } = [];
}
