using Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class VLANDTO : IMapWith<VLAN>
{
  public int VLANNumber { get; set; } // 10
  public string VLANName { get; set; } = string.Empty; // "VLAN10"

  void IMapWith<VLAN>.Mapping(Profile profile)
  {
    profile.CreateMap<VLAN, VLANDTO>()
           .ForMember(dest => dest.VLANNumber,
                      opt => opt.MapFrom(src => src.VLANNumber))
           .ForMember(dest => dest.VLANName,
                      opt => opt.MapFrom(src => src.VLANName));
  }
}