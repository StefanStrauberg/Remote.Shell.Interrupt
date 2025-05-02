namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents the possible statuses of a network interface port.
/// </summary>
public enum PortStatus
{
  /// <summary>
  /// The port is active and operational.
  /// </summary>
  up = 1,

  /// <summary>
  /// The port is inactive or turned off.
  /// </summary>
  down = 2,

  /// <summary>
  /// The port is undergoing testing or diagnostics.
  /// </summary>
  testing = 3,

  /// <summary>
  /// The port status is unknown.
  /// </summary>
  unknown = 4,

  /// <summary>
  /// The port is temporarily inactive but may resume operation.
  /// </summary>
  dormant = 5,

  /// <summary>
  /// The port is physically absent or not detected.
  /// </summary>
  notPresent = 6,

  /// <summary>
  /// The port is down due to issues in the lower-layer protocols.
  /// </summary>
  lowerLayerDown = 7
}
