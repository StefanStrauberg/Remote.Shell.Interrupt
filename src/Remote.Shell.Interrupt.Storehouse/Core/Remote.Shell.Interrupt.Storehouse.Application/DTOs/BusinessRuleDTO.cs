namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs;

public class BusinessRuleDTO
{
  public Guid Id { get; private set; }
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string Condition { get; set; } = string.Empty; // x.Name == Huawei
  public int[] Branch { get; set; } = []; // [1, 3, 15]
  public int SequenceNumber { get; set; } // 15
  public Assignment? Assignment { get; set; } // Represent what to do
}
