namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

/// <summary>
/// Represents a VLAN configuration entity associated with a client and COD.
/// </summary>
public class SPRVlan : BaseEntity
{
  /// <summary>
  /// Gets or sets the unique VLAN identifier.
  /// </summary>
  public int IdVlan { get; set; }
  
  /// <summary>
  /// Gets or sets the unique identifier of the client associated with the VLAN.
  /// </summary>
  public int IdClient { get; set; }

  /// <summary>
  /// Indicates whether the VLAN is used by the client.
  /// </summary>
  public bool UseClient { get; set; }

  /// <summary>
  /// Indicates whether the VLAN is used by the COD.
  /// </summary>
  public bool UseCOD { get; set; }
}
