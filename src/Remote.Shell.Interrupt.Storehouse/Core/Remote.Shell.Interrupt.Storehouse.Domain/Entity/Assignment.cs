namespace Remote.Shell.Interrupt.Storehouse.Domain.Entity;

public class Assignment : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces
  public TypeOfRequest TypeOfRequest { get; set; } // Walk
  public string TargetFieldName { get; set; } = string.Empty; // Interfaces
  public string OID { get; set; } = string.Empty; // sysName - 1.3.6.1.2.1.1.5
}
