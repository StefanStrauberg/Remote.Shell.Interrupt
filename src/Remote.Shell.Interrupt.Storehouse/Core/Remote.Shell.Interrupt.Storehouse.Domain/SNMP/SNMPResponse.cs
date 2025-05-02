namespace Remote.Shell.Interrupt.Storehouse.Domain.SNMP;

/// <summary>
/// Represents a response from an SNMP query, containing an Object Identifier (OID) and its associated data.
/// </summary>
public class SNMPResponse
{
  /// <summary>
  /// Gets or sets the Object Identifier (OID) for the SNMP response.
  /// </summary>
  public string OID { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the data returned for the specified OID.
  /// </summary>
  public string Data { get; set; } = string.Empty;
}