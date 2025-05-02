namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

/// <summary>
/// Defines the various types of network devices supported in the system.
/// </summary>
public enum TypeOfNetworkDevice
{
  /// <summary>
  /// Huawei network device.
  /// </summary>
  Huawei,

  /// <summary>
  /// Juniper network device.
  /// </summary>
  Juniper,

  /// <summary>
  /// Extreme Networks device.
  /// </summary>
  Extreme,

  /// <summary>
  /// Cisco network device.
  /// </summary>
  Cisco,

  /// <summary>
  /// FortiGate network device.
  /// </summary>
  FortiGate
}