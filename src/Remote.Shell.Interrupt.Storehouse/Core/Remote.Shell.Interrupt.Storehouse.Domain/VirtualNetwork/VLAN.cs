namespace Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;
public class VLAN : BaseEntity
{
  public int VLANNumber { get; set; } // 10
  public string VLANName { get; set; } = string.Empty; // "VLAN10"
}