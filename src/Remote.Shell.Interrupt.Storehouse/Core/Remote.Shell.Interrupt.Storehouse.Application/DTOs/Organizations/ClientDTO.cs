namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class ClientDTO : BaseClientDTO, IMapWith<Client>
{
  public Guid Id { get; set; }

   void IMapWith<Client>.Mapping(Profile profile)
   {
        profile.CreateMap<Client, ClientDTO>()
               .ForMember(dest => dest.Id, 
                          opt => opt.MapFrom(src => src.Id));
   }
}