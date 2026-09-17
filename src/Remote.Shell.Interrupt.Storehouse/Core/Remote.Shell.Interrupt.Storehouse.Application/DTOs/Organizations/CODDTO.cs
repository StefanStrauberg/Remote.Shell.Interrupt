namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class CODDTO : IRegister
{
  public Guid Id { get; set; }
  public string NameCOD { get; set; } = string.Empty;
  public string Telephone { get; set; } = string.Empty;
  public string Email1 { get; set; } = string.Empty;
  public string Email2 { get; set; } = string.Empty;
  public string Contact { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Region { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<COD, CODDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.NameCOD,
               src => src.NameCOD)
          .Map(dest => dest.Telephone,
               src => src.Telephone)
          .Map(dest => dest.Email1,
               src => src.Email1)
          .Map(dest => dest.Email2,
               src => src.Email2)
          .Map(dest => dest.Contact,
               src => src.Contact)
          .Map(dest => dest.Description,
               src => src.Description)
          .Map(dest => dest.Region,
               src => src.Region);
  }
}