namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class ClientCod : BaseEntity
{
  public int IdClient { get; set; }
  public string Name { get; set; } = string.Empty;
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string ContactT { get; set; } = string.Empty;
  public string TelephoneT { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public bool Working { get; set; }
  public string EmailT { get; set; } = string.Empty;
  public int IdCOD { get; set; }
  public int IdTPlan { get; set; }
  public string History { get; set; } = string.Empty;
  public bool AntiDDOS { get; set; }
}
