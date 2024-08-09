namespace Remote.Shell.Interrupt.Storehouse.Domain.Entity;

public class OIDRepository : BaseEntity
{
  public string Vendor { get; set; } = string.Empty; // Huawei
  public Dictionary<string, string> OIDs { get; set; } = []; // sysName - 1.3.6.1.2.1.1.5
}
