namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class ScriptGlobals
{
  // BaseEntity properties
  public Guid Id { get; set; }
  public DateTime Created { get; set; }
  public DateTime Modified { get; set; }

  // NetworkDevice properties
  public string? Host { get; set; }
  public string NetworkDeviceName { get; set; } = string.Empty;
  public string GeneralInformation { get; set; } = string.Empty;
  public ICollection<Port> PortsOfNetworkDevice { get; set; } = [];

  // // Port properties
  // public int InterfaceNumber { get; set; } // 1
  // public string PortName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  // public PortType InterfaceType { get; set; } // "Ethernet"
  // public PortStatus InterfaceStatus { get; set; } // "Up"
  // public ulong SpeedOfPort { get; set; } // "1 Gbps"
  // public ICollection<VLAN> VLANsOfPort { get; set; } = [];
  // public IDictionary<MACAddress, IPAddress>? ARPTableOfPort { get; set; }

  // // VLAN properties
  // public int VLANNumber { get; set; } // 10
  // public string VLANName { get; set; } = string.Empty; // "VLAN10"
  // public ICollection<Port> InterfacesOfVLAN { get; set; } = [];
  // public Guid NetworkDeviceId { get; set; }
}