namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class TerminatedNetworkEntityDTO : IMapWith<TerminatedNetworkEntity>
{
  public string IPAddress { get; set; } = string.Empty;
  public string Netmask { get; set; } = string.Empty;

  void IMapWith<TerminatedNetworkEntity>.Mapping(Profile profile)
  {
    profile.CreateMap<TerminatedNetworkEntity, TerminatedNetworkEntityDTO>()
           .ForMember(dest => dest.IPAddress,
                      opt => opt.MapFrom(src => src.IPAddress))
           .ForMember(dest => dest.Netmask,
                      opt => opt.MapFrom(src => src.Netmask));
  }
}