namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class Port : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string InterfaceName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public PortType InterfaceType { get; set; } // "Ethernet"
  public PortStatus InterfaceStatus { get; set; } // "Up"
  public ulong InterfaceSpeed { get; set; } // "1 Gbps"
  public string MACAddress { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  public ICollection<ARPEntity> ARPTableOfInterface { get; set; } = [];
  public ICollection<MACEntity> MACTable { get; set; } = [];
  public ICollection<TerminatedNetworkEntity> NetworkTableOfInterface { get; set; } = [];
  public ICollection<VLAN> VLANs { get; set; } = [];

  // Добавляем свойство для агрегации
  public ICollection<Port> AggregatedPorts { get; set; } = [];

  public Guid NetworkDeviceId { get; set; }
  public NetworkDevice NetworkDevice { get; set; } = null!;

  // Свойство для указания родительского порта
  public Guid? ParentPortId { get; set; }
  public Port? ParentPort { get; set; }
}
