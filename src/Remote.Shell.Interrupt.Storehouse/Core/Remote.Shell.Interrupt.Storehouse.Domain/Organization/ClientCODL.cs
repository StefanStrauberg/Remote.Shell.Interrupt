namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class ClientCODL : BaseEntity
{
  public int IdClient { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? ContactC { get; set; } = string.Empty;
  public string? TelephoneC { get; set; } = string.Empty;
  public string? ContactT { get; set; } = string.Empty;
  public string? TelephoneT { get; set; } = string.Empty;
  public string? EmailC { get; set; } = string.Empty;
  public bool Working { get; set; }
  public string? EmailT { get; set; } = string.Empty;
  public string? History { get; set; } = string.Empty;
  public bool AntiDDOS { get; set; }
  public int Id_COD { get; set; }
  public CODL COD { get; set; } = null!;
  public int? Id_TfPlan { get; set; } = null!;
  public TfPlanL? TfPlanL { get; set; } = null!;

  public override bool Equals(object obj)
  {
    if (obj is ClientCODL other)
      return this.IdClient == other.IdClient;

    return false;
  }

  public override int GetHashCode()
  {
    return IdClient.GetHashCode();
  }
}
