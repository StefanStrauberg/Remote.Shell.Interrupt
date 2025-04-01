namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class DetailClientDTO : BaseClientDTO, IMapWith<Client>
{
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public string History { get; set; } = string.Empty;
  public int Id_COD { get; set; }
  public CODDTO COD { get; set; } = null!;
  public int? Id_TPlan { get; set; } = null!;
  public TfPlanDTO? TfPlan { get; set; } = null!;
  public List<SPRVlanDTO> SPRVlans { get; set; } = [];

  void IMapWith<Client>.Mapping(Profile profile)
  {
    profile.CreateMap<Client, DetailClientDTO>()
           .ForMember(dest => dest.ContactC,
                      opt => opt.MapFrom(src => src.ContactC))
           .ForMember(dest => dest.TelephoneC,
                      opt => opt.MapFrom(src => src.TelephoneC))
           .ForMember(dest => dest.EmailC,
                      opt => opt.MapFrom(src => src.EmailC))
           .ForMember(dest => dest.History,
                      opt => opt.MapFrom(src => src.History))
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
