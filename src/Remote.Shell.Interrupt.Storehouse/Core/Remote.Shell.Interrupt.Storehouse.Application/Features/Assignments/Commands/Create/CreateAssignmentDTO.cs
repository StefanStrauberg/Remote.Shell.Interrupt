namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Create;

public class CreateAssignmentDTO : IMapWith<Assignment>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;

  void IMapWith<Assignment>.Mapping(Profile profile)
  {
    profile.CreateMap<CreateAssignmentDTO, Assignment>()
           .ForMember(assignment => assignment.Id,
                      opt => opt.MapFrom(createAssignmentDTO => createAssignmentDTO.Id))
           .ForMember(assignment => assignment.Name,
                      opt => opt.MapFrom(createAssignmentDTO => createAssignmentDTO.Name))
           .ForMember(assignment => assignment.TypeOfRequest,
                      opt => opt.MapFrom(createAssignmentDTO => createAssignmentDTO.TypeOfRequest))
           .ForMember(assignment => assignment.TargetFieldName,
                      opt => opt.MapFrom(createAssignmentDTO => createAssignmentDTO.TargetFieldName))
           .ForMember(assignment => assignment.OID,
                      opt => opt.MapFrom(createAssignmentDTO => createAssignmentDTO.OID));
  }
}
