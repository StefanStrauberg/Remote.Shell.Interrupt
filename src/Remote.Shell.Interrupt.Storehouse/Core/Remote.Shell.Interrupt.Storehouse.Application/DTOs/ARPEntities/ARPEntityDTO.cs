namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.ARPEntities;

public class ARPEntityDTO : IRegister
{
  public string MAC { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<ARPEntity, ARPEntityDTO>()
          .Map(dest => dest.MAC,
               src => src.MAC)
          .Map(dest => dest.IPAddress,
               src => src.IPAddress);
  }
}