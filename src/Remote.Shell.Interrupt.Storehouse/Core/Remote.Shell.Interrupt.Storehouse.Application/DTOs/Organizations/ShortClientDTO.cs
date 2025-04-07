namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class ShortClientDTO : BaseClientDTO, IMapWith<Client>
{
  public Guid Id { get; set; }

   void IMapWith<Client>.Mapping(Profile profile)
   {
        profile.CreateMap<Client, ShortClientDTO>()
               .ForMember(dest => dest.Id, 
                          opt => opt.MapFrom(src => src.Id));
   }
}