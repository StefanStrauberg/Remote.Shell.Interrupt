using System.Text;

namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public Expression<Func<object, bool>>? Condition { get; set; } // x.Name == Huawei
  public int[] Branch { get; set; } = []; // [1, 3, 15]
  public int SequenceNumber { get; set; } // 15
  public Assignment? Assignment { get; set; } // Represent what to do
}