namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class BaseClientCODDTO : IMapWith<ClientCODL>
{
    public Guid Id { get; set; }
    public int IdClient { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactT { get; set; } = string.Empty;
    public string TelephoneT { get; set; } = string.Empty;
    public string EmailT { get; set; } = string.Empty;
    public bool Working { get; set; }
    public bool AntiDDOS { get; set; }

    void IMapWith<ClientCODL>.Mapping(Profile profile)
  {
    profile.CreateMap<ClientCODL, BaseClientCODDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.IdClient,
                      opt => opt.MapFrom(src => src.IdClient))
           .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.ContactT,
                      opt => opt.MapFrom(src => src.ContactT))
           .ForMember(dest => dest.TelephoneT,
                      opt => opt.MapFrom(src => src.TelephoneT))
           .ForMember(dest => dest.EmailT,
                      opt => opt.MapFrom(src => src.EmailT))
           .ForMember(dest => dest.Working,
                      opt => opt.MapFrom(src => src.Working))
           .ForMember(dest => dest.AntiDDOS,
                      opt => opt.MapFrom(src => src.AntiDDOS));
  }
}
