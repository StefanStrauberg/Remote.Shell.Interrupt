namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class BaseClientDTO : IRegister
{
    public int IdClient { get; set; }
    public string NrDogovor { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContactT { get; set; } = string.Empty;
    public string TelephoneT { get; set; } = string.Empty;
    public string EmailT { get; set; } = string.Empty;
    public bool Working { get; set; }
    public bool AntiDDOS { get; set; }

    void IRegister.Register(TypeAdapterConfig config)
    {
      config.NewConfig<Client, BaseClientDTO>()
            .Map(dest => dest.IdClient,
                 src => src.IdClient)
            .Map(dest => dest.NrDogovor,
                 src => src.NrDogovor)
            .Map(dest => dest.Name,
                 src => src.Name)
            .Map(dest => dest.ContactT,
                 src => src.ContactT)
            .Map(dest => dest.TelephoneT,
                 src => src.TelephoneT)
            .Map(dest => dest.EmailT,
                 src => src.EmailT)
            .Map(dest => dest.Working,
                 src => src.Working)
            .Map(dest => dest.AntiDDOS,
                 src => src.AntiDDOS);
    }
}
