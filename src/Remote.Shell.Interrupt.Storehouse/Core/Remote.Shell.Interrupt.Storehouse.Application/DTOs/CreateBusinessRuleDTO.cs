namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs;

public class CreateBusinessRuleDTO
{
  public string Name { get; set; } = string.Empty;
  public string Condition { get; set; } = string.Empty; // TODO Convert from string to Expression<Func<BusinessRule, bool>> Condition
  public int[] Branch { get; set; } = [];
  public Assignment? Assignment { get; set; }
};
