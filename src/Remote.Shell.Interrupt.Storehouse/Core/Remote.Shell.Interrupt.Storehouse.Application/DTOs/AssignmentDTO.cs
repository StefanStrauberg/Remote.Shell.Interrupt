namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs;

public class AssignmentDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;
}
