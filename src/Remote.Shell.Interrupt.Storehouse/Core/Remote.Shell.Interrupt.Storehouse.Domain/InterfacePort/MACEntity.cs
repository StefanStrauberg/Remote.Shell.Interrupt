namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class MACEntity : BaseEntity
{
  public string MACAddress { get; set; } = string.Empty;
  public Port Port { get; set; } = null!;
  public Guid PortId { get; set; }
}