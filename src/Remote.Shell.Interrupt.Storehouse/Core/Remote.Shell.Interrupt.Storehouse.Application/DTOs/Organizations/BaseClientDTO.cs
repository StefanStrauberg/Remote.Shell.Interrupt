namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class BaseClientDTO : IMapWith<Client>
{
    public int IdClient { get; set; }
    public string NrDogovor { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContactT { get; set; } = string.Empty;
    public string TelephoneT { get; set; } = string.Empty;
    public string EmailT { get; set; } = string.Empty;
    public bool Working { get; set; }
    public bool AntiDDOS { get; set; }

    void IMapWith<Client>.Mapping(Profile profile)
    {
      profile.CreateMap<Client, BaseClientDTO>()
             .ForMember(dest => dest.IdClient,
                        opt => opt.MapFrom(src => src.IdClient))
             .ForMember(dest => dest.NrDogovor,
                      opt => opt.MapFrom(src => src.NrDogovor))
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
