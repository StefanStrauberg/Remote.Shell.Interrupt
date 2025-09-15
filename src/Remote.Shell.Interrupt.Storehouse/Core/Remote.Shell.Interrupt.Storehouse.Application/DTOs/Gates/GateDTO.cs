namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class GateDTO : BaseGateDTO, IMapWith<Gate>
{
  public Guid Id { get; set; }
  public string IPAddress { get; set; } = string.Empty;

  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<Gate, GateDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.IPAddress,
                      opt => opt.MapFrom(src => ConvertLongIPAddressToString.Handle(src.IPAddress)));
  }
}
