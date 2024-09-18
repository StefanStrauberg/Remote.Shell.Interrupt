namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces 
  public TypeOfNetworkDevice? Vendor { get; set; } // Huawei or null
  public bool IsRoot { get; set; }

  public Guid? ParentId { get; set; }
  public BusinessRule? Parent { get; set; }

  public Guid AssignmentId { get; set; }
  public Assignment Assignment { get; set; } = null!;

  public List<BusinessRule> Children { get; set; } = [];
}
