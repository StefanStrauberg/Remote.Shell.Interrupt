namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class UpdateGateDTO : GateDTO, IRegister
{
  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<UpdateGateDTO, Gate>()
          .Map(dest => dest.IPAddress,
               src => ConvertStringIPAddressToLong.Handle(src.IPAddress));
  }
}
