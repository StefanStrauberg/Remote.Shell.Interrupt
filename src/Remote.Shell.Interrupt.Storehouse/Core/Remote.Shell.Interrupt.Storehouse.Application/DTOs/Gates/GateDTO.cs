namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class GateDTO : BaseGateDTO, IRegister
{
  public Guid Id { get; set; }
  public string IPAddress { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<Gate, GateDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.IPAddress,
               src => ConvertLongIPAddressToString.Handle(src.IPAddress));
  }
}
