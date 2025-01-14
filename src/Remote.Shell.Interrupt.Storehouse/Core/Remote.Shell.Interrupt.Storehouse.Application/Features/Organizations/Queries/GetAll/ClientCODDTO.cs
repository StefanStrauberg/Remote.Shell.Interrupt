namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class ClientCODDTO : IMapWith<ClientCODL>
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
  public int Id_COD { get; set; }
  public CODDTO COD { get; set; } = null!;
  public int? Id_TPlan { get; set; } = null!;
  public TfPlanDTO? TfPlan { get; set; } = null!;
  public List<SPRVlanDTO> SPRVlans { get; set; } = [];

  void IMapWith<ClientCODL>.Mapping(Profile profile)
  {
    profile.CreateMap<ClientCODL, ClientCODDTO>()
           .ForMember(dest => dest.IdClient,
                      opt => opt.MapFrom(src => src.IdClient))
           .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.ContactC,
                      opt => opt.MapFrom(src => src.ContactC))
           .ForMember(dest => dest.TelephoneC,
                      opt => opt.MapFrom(src => src.TelephoneC))
           .ForMember(dest => dest.ContactT,
                      opt => opt.MapFrom(src => src.ContactT))
           .ForMember(dest => dest.TelephoneT,
                      opt => opt.MapFrom(src => src.TelephoneT))
           .ForMember(dest => dest.EmailC,
                      opt => opt.MapFrom(src => src.EmailC))
           .ForMember(dest => dest.Working,
                      opt => opt.MapFrom(src => src.Working))
           .ForMember(dest => dest.EmailT,
                      opt => opt.MapFrom(src => src.EmailT))
           .ForMember(dest => dest.History,
                      opt => opt.MapFrom(src => src.History))
           .ForMember(dest => dest.AntiDDOS,
                      opt => opt.MapFrom(src => src.AntiDDOS))
           .ForMember(dest => dest.Id_COD,
                      opt => opt.MapFrom(src => src.Id_COD))
           .ForMember(dest => dest.COD,
                      opt => opt.MapFrom(src => src.COD))
           .ForMember(dest => dest.Id_TPlan,
                      opt => opt.MapFrom(src => src.Id_TfPlan))
           .ForMember(dest => dest.TfPlan,
                      opt => opt.MapFrom(src => src.TfPlanL));
  }
}
