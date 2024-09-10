namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class Port : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string PortName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public PortType InterfaceType { get; set; } // "Ethernet"
  public PortStatus InterfaceStatus { get; set; } // "Up"
  public ulong SpeedOfPort { get; set; } // "1 Gbps"

  public ICollection<ARPEntity> ARPTableOfPort { get; set; } = [];
  public ICollection<TerminatedNetworkEntity> NetworkTableOfPort { get; set; } = [];
  public ICollection<PortVLAN> PortVLANS { get; set; } = [];

  public Guid NetworkDeviceId { get; set; }
  public NetworkDevice NetworkDevice { get; set; } = null!;
}
