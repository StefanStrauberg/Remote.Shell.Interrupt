namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Update;

public class UpdateAssignmentDTO : IMapWith<Assignment>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;

  void IMapWith<Assignment>.Mapping(Profile profile)
  {
    profile.CreateMap<UpdateAssignmentDTO, Assignment>()
           .ForMember(assignment => assignment.Id,
                      opt => opt.MapFrom(updateAssignmentDTO => updateAssignmentDTO.Id))
           .ForMember(assignment => assignment.Name,
                      opt => opt.MapFrom(updateAssignmentDTO => updateAssignmentDTO.Name))
           .ForMember(assignment => assignment.TypeOfRequest,
                      opt => opt.MapFrom(updateAssignmentDTO => updateAssignmentDTO.TypeOfRequest))
           .ForMember(assignment => assignment.TargetFieldName,
                      opt => opt.MapFrom(updateAssignmentDTO => updateAssignmentDTO.TargetFieldName))
           .ForMember(assignment => assignment.OID,
                      opt => opt.MapFrom(updateAssignmentDTO => updateAssignmentDTO.OID));
  }
}
