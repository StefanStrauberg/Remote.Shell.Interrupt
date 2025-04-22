namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Vlans;

public class VLANDTO : IMapWith<VLAN>
{
  public int VLANTag { get; set; } // 10
  public string VLANName { get; set; } = string.Empty; // "VLAN10"

  void IMapWith<VLAN>.Mapping(Profile profile)
  {
    profile.CreateMap<VLAN, VLANDTO>()
           .ForMember(dest => dest.VLANTag,
                      opt => opt.MapFrom(src => src.VLANTag))
           .ForMember(dest => dest.VLANName,
                      opt => opt.MapFrom(src => src.VLANName));
  }
}