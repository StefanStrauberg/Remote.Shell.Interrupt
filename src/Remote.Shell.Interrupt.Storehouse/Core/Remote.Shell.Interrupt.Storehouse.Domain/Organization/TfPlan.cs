namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class TfPlan : BaseEntity
{
  public int IdTfPlan { get; set; }
  public string NameTfPlan { get; set; } = string.Empty;
  public string? DescTfPlan { get; set; } = string.Empty;
}