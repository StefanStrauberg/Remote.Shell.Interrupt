namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class CreateGateDTO : BaseGateDTO, IRegister
{
  public string IPAddress { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<CreateGateDTO, Gate>()
          .Map(dest => dest.IPAddress,
               src => ConvertStringIPAddressToLong.Handle(src.IPAddress));
  }
}
