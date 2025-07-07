namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

/// <summary>
/// Defines a unit of work interface for managing network-related repository operations.
/// </summary>
public interface INetDevUnitOfWork : IUnitOfWork
{
  /// <summary>
  /// Gets the repository for network device entities.
  /// </summary>
  INetworkDeviceRepository NetworkDevices { get; }

  /// <summary>
  /// Gets the repository for VLAN entities.
  /// </summary>
  IVLANRepository VLANs { get; }

  /// <summary>
  /// Gets the repository for port entities.
  /// </summary>
  IPortRepository Ports { get; }

  /// <summary>
  /// Gets the repository for ARPEntity records.
  /// </summary>
  IARPEntityRepository ARPEntities { get; }

  /// <summary>
  /// Gets the repository for MACEntity records.
  /// </summary>
  IMACEntityRepository MACEntities { get; }

  /// <summary>
  /// Gets the repository for terminated network entities.
  /// </summary>
  ITerminatedNetworkEntityRepository TerminatedNetworkEntities { get; }
}