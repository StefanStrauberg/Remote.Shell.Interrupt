namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class CODDTO : IMapWith<CODL>
{
  public int Id { get; set; }
  public string NameCOD { get; set; } = string.Empty;
  public string Telephone { get; set; } = string.Empty;
  public string Email1 { get; set; } = string.Empty;
  public string Email2 { get; set; } = string.Empty;
  public string Contact { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Region { get; set; } = string.Empty;

  void IMapWith<CODL>.Mapping(Profile profile)
  {
    profile.CreateMap<CODL, CODDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.NameCOD,
                      opt => opt.MapFrom(src => src.NameCOD))
           .ForMember(dest => dest.Telephone,
                      opt => opt.MapFrom(src => src.Telephone))
           .ForMember(dest => dest.Email1,
                      opt => opt.MapFrom(src => src.Email1))
           .ForMember(dest => dest.Email2,
                      opt => opt.MapFrom(src => src.Email2))
           .ForMember(dest => dest.Contact,
                      opt => opt.MapFrom(src => src.Contact))
           .ForMember(dest => dest.Description,
                      opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.Region,
                      opt => opt.MapFrom(src => src.Region));
  }
}