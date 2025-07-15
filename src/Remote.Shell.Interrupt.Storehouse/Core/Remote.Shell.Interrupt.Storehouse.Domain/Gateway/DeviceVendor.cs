namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

/// <summary>
/// Represents a vendor or manufacturer of a network device.
/// </summary>
public class DeviceVendor : BaseEntity
{
  /// <summary>
  /// Gets or sets the name of the device vendor (e.g., Cisco, Juniper, Extreme).
  /// </summary>
  public string Name { get; set; } = string.Empty;
}
