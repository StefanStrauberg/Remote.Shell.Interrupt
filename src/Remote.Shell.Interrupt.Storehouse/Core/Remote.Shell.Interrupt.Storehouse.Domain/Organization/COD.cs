namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

/// <summary>
/// Represents a Data Center entity.
/// </summary>
public class COD : BaseEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the COD entity.
  /// </summary>
  public int IdCOD { get; set; }

  /// <summary>
  /// Gets or sets the name of the COD entity.
  /// </summary>
  public string NameCOD { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the contact telephone number.
  /// </summary>
  public string? Telephone { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the primary email address.
  /// </summary>
  public string? Email1 { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the secondary email address.
  /// </summary>
  public string? Email2 { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the contact person associated with the COD entity.
  /// </summary>
  public string? Contact { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the COD entity.
  /// </summary>
  public string? Description { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the region where the COD entity operates.
  /// </summary>
  public string? Region { get; set; } = string.Empty;
}
