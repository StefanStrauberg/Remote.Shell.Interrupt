namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class ARPEntity : BaseEntity
{
  public string MAC { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;

  public Guid PortId { get; set; }
}
