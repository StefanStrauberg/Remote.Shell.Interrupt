namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class DetailClientDTO : BaseClientDTO, IMapWith<Client>
{
  public Guid Id { get; set; }
  public DateTime Dat1 { get; set; }
  public DateTime Dat2 { get; set; }
  public string Prim1 { get; set; } = string.Empty;
  public string Prim2 { get; set; } = string.Empty;
  public string Nik { get; set; } =string.Empty;
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
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Dat1,
                      opt => opt.MapFrom(src => src.Dat1))
           .ForMember(dest => dest.Dat2,
                      opt => opt.MapFrom(src => src.Dat2))
           .ForMember(dest => dest.Prim1,
                      opt => opt.MapFrom(src => src.Prim1))
           .ForMember(dest => dest.Prim2,
                      opt => opt.MapFrom(src => src.Prim2))
           .ForMember(dest => dest.Nik,
                      opt => opt.MapFrom(src => src.Nik))
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
