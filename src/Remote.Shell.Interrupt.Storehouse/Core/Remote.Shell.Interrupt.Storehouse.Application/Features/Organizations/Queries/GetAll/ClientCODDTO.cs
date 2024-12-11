namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class ClientCODDTO : IMapWith<ClientCOD>
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Contact { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  void IMapWith<ClientCOD>.Mapping(Profile profile)
  {
    profile.CreateMap<ClientCOD, ClientCODDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Contact,
                      opt => opt.MapFrom(src => src.Contact))
           .ForMember(dest => dest.Email,
                      opt => opt.MapFrom(src => src.Email));
  }
}
