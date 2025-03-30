namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;

public class GateDTO : BaseGateDTO, IMapWith<Gate>
{
  public Guid Id { get; set; }
  
  void IMapWith<Gate>.Mapping(Profile profile)
  {
    profile.CreateMap<Gate, GateDTO>()
           .ForMember(gateDTO => gateDTO.Id,
                      opt => opt.MapFrom(gate => gate.Id));
  }
}
