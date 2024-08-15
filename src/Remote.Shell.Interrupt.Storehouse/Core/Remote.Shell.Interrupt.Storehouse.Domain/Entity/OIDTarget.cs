namespace Remote.Shell.Interrupt.Storehouse.Domain.Entity;

public class OIDTarget : BaseEntity
{
  public string OIDTargetName { get; set; } = string.Empty;
  public TargetAction TargetAction { get; set; }
  public string Target { get; set; } = string.Empty; // Huawei
  public string OID { get; set; } = string.Empty; // sysName - 1.3.6.1.2.1.1.5
}
