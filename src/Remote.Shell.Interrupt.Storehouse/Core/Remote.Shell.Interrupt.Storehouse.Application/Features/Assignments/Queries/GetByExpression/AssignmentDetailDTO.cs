namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetByExpression;

public class AssignmentDetailDTO : IMapWith<Assignment>
{
  public Guid Id { get; set; }
  public DateTime Created { get; set; }
  public DateTime Modified { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;

  void IMapWith<Assignment>.Mapping(Profile profile)
  {
    profile.CreateMap<Assignment, AssignmentDetailDTO>()
           .ForMember(src => src.Id,
                      opt => opt.MapFrom(dst => dst.Id))
           .ForMember(src => src.Created,
                      opt => opt.MapFrom(dst => dst.Created))
           .ForMember(src => src.Modified,
                      opt => opt.MapFrom(dst => dst.Modified))
           .ForMember(src => src.Name,
                      opt => opt.MapFrom(dst => dst.Name))
           .ForMember(src => src.TypeOfRequest,
                      opt => opt.MapFrom(dst => dst.TypeOfRequest))
           .ForMember(src => src.TargetFieldName,
                      opt => opt.MapFrom(dst => dst.TargetFieldName))
           .ForMember(src => src.OID,
                      opt => opt.MapFrom(dst => dst.OID));
  }
}
