namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public List<int> Branch { get; set; } = []; // [1, 3, 15]
  public int SequenceNumber { get; set; } // 15
  public Guid? AssignmentId { get; set; } // Represent what to do
}