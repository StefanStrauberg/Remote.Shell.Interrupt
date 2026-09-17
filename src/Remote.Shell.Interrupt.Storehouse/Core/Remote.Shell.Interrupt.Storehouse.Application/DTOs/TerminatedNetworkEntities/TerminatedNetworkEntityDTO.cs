namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.TerminatedNetworkEntities;

public class TerminatedNetworkEntityDTO : IRegister
{
  public string NetworkAddress { get; set; } = string.Empty;
  public string Netmask { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<TerminatedNetworkEntity, TerminatedNetworkEntityDTO>()
          .Map(dest => dest.NetworkAddress,
               src => ConvertLongIPAddressToString.Handle(src.NetworkAddress))
          .Map(dest => dest.Netmask,
               src => ConvertLongIPAddressToString.Handle(src.Netmask));
  }
}