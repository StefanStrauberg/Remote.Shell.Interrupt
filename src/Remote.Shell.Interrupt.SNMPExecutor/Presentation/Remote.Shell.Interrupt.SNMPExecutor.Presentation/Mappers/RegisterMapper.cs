namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation.Mappers;

public class RegisterMapper : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<SNMPGetWalkRequest, SNMPGetCommand>()
          .Map(dst => dst.Host, src => src.Host)
          .Map(dst => dst.Community, src => src.Community)
          .Map(dst => dst.OID, src => src.OID);

    config.NewConfig<SNMPGetWalkRequest, SNMPWalkCommand>()
          .Map(dst => dst.Host, src => src.Host)
          .Map(dst => dst.Community, src => src.Community)
          .Map(dst => dst.OID, src => src.OID);
  }
}
