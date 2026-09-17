namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class DetailClientDTO : BaseClientDTO, IRegister
{
  public Guid Id { get; set; }
  public DateTime? Dat1 { get; set; }
  public DateTime? Dat2 { get; set; }
  public string Prim1 { get; set; } = string.Empty;
  public string Prim2 { get; set; } = string.Empty;
  public string Nik { get; set; } = string.Empty;
  public string ContactC { get; set; } = string.Empty;
  public string TelephoneC { get; set; } = string.Empty;
  public string EmailC { get; set; } = string.Empty;
  public string History { get; set; } = string.Empty;
  public int Id_COD { get; set; }
  public CODDTO COD { get; set; } = null!;
  public int? Id_TPlan { get; set; }
  public TfPlanDTO? TfPlan { get; set; }
  public List<SPRVlanDTO> SPRVlans { get; set; } = [];

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<Client, DetailClientDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.Dat1,
               src => src.Dat1)
          .Map(dest => dest.Dat2,
               src => src.Dat2)
          .Map(dest => dest.Prim1,
               src => src.Prim1)
          .Map(dest => dest.Prim2,
               src => src.Prim2)
          .Map(dest => dest.Nik,
               src => src.Nik)
          .Map(dest => dest.ContactC,
               src => src.ContactC)
          .Map(dest => dest.TelephoneC,
               src => src.TelephoneC)
          .Map(dest => dest.EmailC,
               src => src.EmailC)
          .Map(dest => dest.History,
               src => src.History)
          .Map(dest => dest.Id_COD,
               src => src.Id_COD)
          .Map(dest => dest.COD,
               src => src.COD)
          .Map(dest => dest.Id_TPlan,
               src => src.Id_TfPlan)
          .Map(dest => dest.TfPlan,
               src => src.TfPlan);
  }
}
