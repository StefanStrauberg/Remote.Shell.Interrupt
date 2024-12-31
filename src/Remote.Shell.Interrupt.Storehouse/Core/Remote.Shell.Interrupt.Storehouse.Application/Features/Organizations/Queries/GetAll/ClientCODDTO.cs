namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class ClientCODDTO : IMapWith<ClientCodR>
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
  public int IdCOD { get; set; }
  public CODDTO COD { get; set; } = null!;
  public int IdTPlan { get; set; }
  public TfPlanDTO TfPlan { get; set; } = null!;

  void IMapWith<ClientCodR>.Mapping(Profile profile)
  {
    profile.CreateMap<ClientCodR, ClientCODDTO>()
           .ForMember(dest => dest.IdClient,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => src.Name.TrimEnd()))
           .ForMember(dest => dest.ContactC,
                      opt => opt.MapFrom(src => src.ContactC.TrimEnd()))
           .ForMember(dest => dest.TelephoneC,
                      opt => opt.MapFrom(src => src.TelephoneC.TrimEnd()))
           .ForMember(dest => dest.ContactT,
                      opt => opt.MapFrom(src => src.ContactT.TrimEnd()))
           .ForMember(dest => dest.TelephoneT,
                      opt => opt.MapFrom(src => src.TelephoneT.TrimEnd()))
           .ForMember(dest => dest.EmailC,
                      opt => opt.MapFrom(src => src.EmailC.TrimEnd()))
           .ForMember(dest => dest.Working,
                      opt => opt.MapFrom(src => src.Working))
           .ForMember(dest => dest.EmailT,
                      opt => opt.MapFrom(src => src.EmailT.TrimEnd()))
           .ForMember(dest => dest.History,
                      opt => opt.MapFrom(src => src.History.TrimEnd()))
           .ForMember(dest => dest.AntiDDOS,
                      opt => opt.MapFrom(src => src.AntiDDOS))
           .ForMember(dest => dest.IdCOD,
                      opt => opt.MapFrom(src => src.IdCOD))
           .ForMember(dest => dest.COD,
                      opt => opt.MapFrom(src => src.COD))
           .ForMember(dest => dest.IdTPlan,
                      opt => opt.MapFrom(src => src.TfPlan))
           .ForMember(dest => dest.TfPlan,
                      opt => opt.MapFrom(src => src.TfPlan));
  }
}
