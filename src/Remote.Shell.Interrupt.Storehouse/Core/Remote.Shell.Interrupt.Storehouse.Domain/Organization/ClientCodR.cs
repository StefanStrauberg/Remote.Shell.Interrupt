namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class ClientCodR
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string ContactT { get; set; } = string.Empty;
  public string TelephoneT { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public bool Working { get; set; }
  public string EmailT { get; set; } = string.Empty;
  public string History { get; set; } = string.Empty;
  public bool AntiDDOS { get; set; }
  public int IdCOD { get; set; }
  public CODR COD { get; set; } = null!;
  public int IdTfPlan { get; set; }
  public TfPlanR TfPlan { get; set; } = null!;
}
