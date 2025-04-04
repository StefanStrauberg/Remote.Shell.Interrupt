namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class RemoteClient
{
  public int IdClient { get; set; }
  public DateTime? Dat1 { get; set; }
  public DateTime? Dat2 { get; set; }
  public string? Prim1 { get; set; } = string.Empty;
  public string? Prim2 { get; set; } = string.Empty;
  public string? Nik { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string NrDogovor { get; set; } = string.Empty;
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string ContactT { get; set; } = string.Empty;
  public string TelephoneT { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public bool Working { get; set; }
  public string EmailT { get; set; } = string.Empty;
  public string History { get; set; } = string.Empty;
  public bool AntiDDOS { get; set; }
  public int Id_COD { get; set; }
  public RemoteCOD COD { get; set; } = null!;
  public int? Id_TfPlan { get; set; }
  public RemoteTfPlan? TfPlan { get; set; } = null!;
}
