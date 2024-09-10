namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class PortVLAN
{
  public Guid PortId { get; set; }
  public Port Port { get; set; } = null!;

  public Guid VLANId { get; set; }
  public VLAN VLAN { get; set; } = null!;
}
