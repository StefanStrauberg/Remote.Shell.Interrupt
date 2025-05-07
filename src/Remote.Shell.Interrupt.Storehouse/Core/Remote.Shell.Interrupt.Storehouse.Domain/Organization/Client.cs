namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

/// <summary>
/// Represents a client entity with contract details and associated configurations.
/// </summary>
public class Client : BaseEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the client.
  /// </summary>
  public int IdClient { get; set; }

  /// <summary>
  /// Gets or sets the first date reference.
  /// </summary>
  public DateTime? Dat1 { get; set; }

  /// <summary>
  /// Gets or sets the second date reference.
  /// </summary>
  public DateTime? Dat2 { get; set; }

  /// <summary>
  /// Gets or sets the first additional remark.
  /// </summary>
  public string? Prim1 { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the second additional remark.
  /// </summary>
  public string? Prim2 { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the client's nickname.
  /// </summary>
  public string? Nik { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the contract number associated with the client.
  /// </summary>
  public string NrDogovor { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the full name of the client.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the comercial contact person.
  /// </summary>
  public string? ContactC { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the telephone number for the comercial contact.
  /// </summary>
  public string? TelephoneC { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the technical contact person.
  /// </summary>
  public string? ContactT { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the telephone number for the technical contact.
  /// </summary>
  public string? TelephoneT { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the email address for the comercial contact.
  /// </summary>
  public string? EmailC { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the client's working status.
  /// </summary>
  public bool Working { get; set; }

  /// <summary>
  /// Gets or sets the email address for the technical contact.
  /// </summary>
  public string? EmailT { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the history log related to the client.
  /// </summary>
  public string? History { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets whether the client has Anti-DDoS protection enabled.
  /// </summary>
  public bool AntiDDOS { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the client's Data Center.
  /// </summary>
  public int Id_COD { get; set; }

  /// <summary>
  /// Gets or sets the Data Center associated with the client.
  /// </summary>
  public COD COD { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier of the client's tariff plan.
  /// </summary>
  public int? Id_TfPlan { get; set; }

  /// <summary>
  /// Gets or sets the tariff plan associated with the client.
  /// </summary>
  public TfPlan? TfPlan { get; set; }

  /// <summary>
  /// Gets or sets the VLAN configuration associated with the client.
  /// </summary>
  public List<SPRVlan> SPRVlans { get; set; } = [];
}
