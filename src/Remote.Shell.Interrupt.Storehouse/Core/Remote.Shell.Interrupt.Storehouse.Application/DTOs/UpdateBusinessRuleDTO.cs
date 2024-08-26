namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs;

public class UpdateBusinessRuleDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Condition { get; set; } = string.Empty;
  public int[] Branch { get; set; } = [];
  public int SequenceNumber { get; set; }
  public Assignment? Assignment { get; set; }
}
