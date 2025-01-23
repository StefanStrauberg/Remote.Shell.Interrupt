namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Update;

public class UpdateGateDTO : IMapWith<Gate>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string IPAddress { get; set; } = string.Empty;
  public string Community { get; set; } = string.Empty;
  public string TypeOfNetworkDevice { get; set; } = string.Empty;

  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<UpdateGateDTO, Gate>()
           .ForMember(gate => gate.Id,
                      opt => opt.MapFrom(gateDTO => gateDTO.Id))
           .ForMember(gate => gate.Name,
                      opt => opt.MapFrom(gateDTO => gateDTO.Name))
           .ForMember(gate => gate.IPAddress,
                      opt => opt.MapFrom(gateDTO => gateDTO.IPAddress))
           .ForMember(gate => gate.Community,
                      opt => opt.MapFrom(gateDTO => gateDTO.Community))
           .ForMember(gate => gate.TypeOfNetworkDevice,
                      opt => opt.MapFrom(gateDTO => Enum.Parse<TypeOfNetworkDevice>(gateDTO.TypeOfNetworkDevice)));
  }
}
