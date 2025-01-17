namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class Gate : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;
  public string Community { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;
}
