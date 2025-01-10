namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class ClientCODDTO : IMapWith<ClientCODR>
{
  public int IdClient { get; set; }
  public string Name { get; set; } = string.Empty;
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string ContactT { get; set; } = string.Empty;
  public string TelephoneT { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public bool Working { get; set; }
  public string EmailT { get; set; } = string.Empty;
  public string History { get; set; } = string.Empty;
  public bool AntiDDOS { get; set; }

  void IMapWith<ClientCODR>.Mapping(Profile profile)
  {
    profile.CreateMap<ClientCODL, ClientCODDTO>()
           .ForMember(dest => dest.IdClient,
                      opt => opt.MapFrom(src => src.IdClient))
           .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => src.Name.TrimEnd()))
           .ForMember(dest => dest.ContactC,
                      opt => opt.MapFrom(src => (src.ContactC ?? "").TrimEnd()))
           .ForMember(dest => dest.TelephoneC,
                      opt => opt.MapFrom(src => (src.TelephoneC ?? "").TrimEnd()))
           .ForMember(dest => dest.ContactT,
                      opt => opt.MapFrom(src => (src.ContactT ?? "").TrimEnd()))
           .ForMember(dest => dest.TelephoneT,
                      opt => opt.MapFrom(src => (src.TelephoneT ?? "").TrimEnd()))
           .ForMember(dest => dest.EmailC,
                      opt => opt.MapFrom(src => (src.EmailC ?? "").TrimEnd()))
           .ForMember(dest => dest.Working,
                      opt => opt.MapFrom(src => src.Working))
           .ForMember(dest => dest.EmailT,
                      opt => opt.MapFrom(src => (src.EmailT ?? "").TrimEnd()))
           .ForMember(dest => dest.History,
                      opt => opt.MapFrom(src => (src.History ?? "").TrimEnd()))
           .ForMember(dest => dest.AntiDDOS,
                      opt => opt.MapFrom(src => src.AntiDDOS));
  }
}
