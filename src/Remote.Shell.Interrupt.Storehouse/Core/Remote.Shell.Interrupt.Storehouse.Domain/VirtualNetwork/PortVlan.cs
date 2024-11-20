namespace Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;

public class PortVlan : BaseEntity
{
  public Guid PortId { get; set; }
  public Guid VLANId { get; set; }
}
