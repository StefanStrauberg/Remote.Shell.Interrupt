namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class CODDTO : IMapWith<CODR>
{
  public int Id { get; set; }
  public string NameCOD { get; set; } = string.Empty;
  public string Telephone { get; set; } = string.Empty;
  public string Email1 { get; set; } = string.Empty;
  public string Email2 { get; set; } = string.Empty;
  public string Contact { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Region { get; set; } = string.Empty;

  void IMapWith<CODR>.Mapping(Profile profile)
  {
    profile.CreateMap<CODR, CODDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.NameCOD,
                      opt => opt.MapFrom(src => src.NameCOD.TrimEnd()))
           .ForMember(dest => dest.Telephone,
                      opt => opt.MapFrom(src => src.Telephone.TrimEnd()))
           .ForMember(dest => dest.Email1,
                      opt => opt.MapFrom(src => src.Email1.TrimEnd()))
           .ForMember(dest => dest.Email2,
                      opt => opt.MapFrom(src => src.Email2.TrimEnd()))
           .ForMember(dest => dest.Contact,
                      opt => opt.MapFrom(src => src.Contact.TrimEnd()))
           .ForMember(dest => dest.Description,
                      opt => opt.MapFrom(src => src.Description.TrimEnd()))
           .ForMember(dest => dest.Region,
                      opt => opt.MapFrom(src => src.Region.TrimEnd()));
  }
}