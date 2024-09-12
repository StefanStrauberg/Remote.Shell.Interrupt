namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class ARPEntityDTO : IMapWith<ARPEntity>
{
  public string MAC { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;

  void IMapWith<ARPEntity>.Mapping(Profile profile)
  {
    profile.CreateMap<ARPEntity, ARPEntityDTO>()
           .ForMember(dest => dest.MAC,
                      opt => opt.MapFrom(src => src.MAC))
           .ForMember(dest => dest.IPAddress,
                      opt => opt.MapFrom(src => src.IPAddress));
  }
}