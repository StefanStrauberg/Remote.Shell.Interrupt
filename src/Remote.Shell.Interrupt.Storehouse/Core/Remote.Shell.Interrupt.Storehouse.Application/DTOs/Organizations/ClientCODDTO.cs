namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class ClientCODDTO : BaseClientCODDTO, IMapWith<ClientCODL>
{
  public Guid Id { get; set; }

   void IMapWith<ClientCODL>.Mapping(Profile profile)
   {
        profile.CreateMap<ClientCODL, ClientCODDTO>()
               .ForMember(dest => dest.Id, 
                          opt => opt.MapFrom(src => src.Id));
   }
}