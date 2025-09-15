namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class CreateGateDTO : BaseGateDTO, IMapWith<Gate>
{
  public string IPAddress { get; set; } = string.Empty;

  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<CreateGateDTO, Gate>()
           .ForMember(dest => dest.IPAddress,
                      opt => opt.MapFrom(src => ConvertStringIPAddressToLong.Handle(src.IPAddress)));
  }
}
