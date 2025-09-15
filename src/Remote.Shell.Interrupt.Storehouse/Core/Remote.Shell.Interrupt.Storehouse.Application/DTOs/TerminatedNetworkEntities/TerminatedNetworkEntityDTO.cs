namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.TerminatedNetworkEntities;

public class TerminatedNetworkEntityDTO : IMapWith<TerminatedNetworkEntity>
{
  public string NetworkAddress { get; set; } = string.Empty;
  public string Netmask { get; set; } = string.Empty;

  void IMapWith<TerminatedNetworkEntity>.Mapping(Profile profile)
  {
    profile.CreateMap<TerminatedNetworkEntity, TerminatedNetworkEntityDTO>()
           .ForMember(dest => dest.NetworkAddress,
                      opt => opt.MapFrom(src => ConvertLongIPAddressToString.Handle(src.NetworkAddress)))
           .ForMember(dest => dest.Netmask,
                      opt => opt.MapFrom(src => ConvertLongIPAddressToString.Handle(src.Netmask)));
  }
}