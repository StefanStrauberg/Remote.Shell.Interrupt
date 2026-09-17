namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class ShortClientDTO : BaseClientDTO, IRegister
{
  public Guid Id { get; set; }

   void IRegister.Register(TypeAdapterConfig config)
   {
        config.NewConfig<Client, ShortClientDTO>()
              .Map(dest => dest.Id,
                   src => src.Id);
   }
}