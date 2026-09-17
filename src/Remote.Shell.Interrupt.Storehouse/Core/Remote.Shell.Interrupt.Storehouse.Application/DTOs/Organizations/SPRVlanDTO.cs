namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class SPRVlanDTO : IRegister
{
  public Guid Id { get; set; }
  public int IdVlan { get; set; }
  public int IdClient { get; set; }
  public bool UseClient { get; set; }
  public bool UseCOD { get; set; }

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<SPRVlan, SPRVlanDTO>()
          .Map(dest => dest.IdVlan,
               src => src.IdVlan)
          .Map(dest => dest.IdClient,
               src => src.IdClient)
          .Map(dest => dest.UseClient,
               src => src.UseClient)
          .Map(dest => dest.UseCOD,
               src => src.UseCOD);
  }
}