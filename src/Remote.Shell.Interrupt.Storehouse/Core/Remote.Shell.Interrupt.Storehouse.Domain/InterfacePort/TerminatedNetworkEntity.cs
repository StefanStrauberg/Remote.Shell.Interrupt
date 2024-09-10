namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class TerminatedNetworkEntity : BaseEntity
{
  public string IPAddress { get; set; } = string.Empty;
  public string Netmask { get; set; } = string.Empty;

  public Guid PortId { get; set; }
  public Port Port { get; set; } = null!;
}
