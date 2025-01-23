namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetAll;

public class GateDTO : IMapWith<Gate>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;
  public string Community { get; set; } = string.Empty;
  public string TypeOfNetworkDevice { get; set; } = string.Empty;

  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<Gate, GateDTO>()
           .ForMember(gateDTO => gateDTO.Id,
                      opt => opt.MapFrom(gate => gate.Id))
           .ForMember(gateDTO => gateDTO.Name,
                      opt => opt.MapFrom(gate => gate.Name))
           .ForMember(gateDTO => gateDTO.IPAddress,
                      opt => opt.MapFrom(gate => gate.IPAddress))
           .ForMember(gateDTO => gateDTO.Community,
                      opt => opt.MapFrom(gate => gate.Community))
           .ForMember(gateDTO => gateDTO.TypeOfNetworkDevice,
                      opt => opt.MapFrom(gate => gate.TypeOfNetworkDevice.ToString()));
  }
}
