namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.AssignmentDTOs;

public class CreateAssignmentDTO
{
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;
}
