namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class UpdateGateDTO : GateDTO, IMapWith<Gate>
{
  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<UpdateGateDTO, Gate>()
           .ForMember(dest => dest.IPAddress,
                      opt => opt.MapFrom(src => ConvertStringIPAddressToLong.Handle(src.IPAddress)));
  }
}
