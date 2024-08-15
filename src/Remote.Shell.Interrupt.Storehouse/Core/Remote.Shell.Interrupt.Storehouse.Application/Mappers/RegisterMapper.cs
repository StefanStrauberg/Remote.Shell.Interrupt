namespace Remote.Shell.Interrupt.Storehouse.Application.Mappers;

public class RegisterMapper : IRegister
{
      public void Register(TypeAdapterConfig config)
      {
            config.NewConfig<UpdateNetworkDeviceCommand, NetworkDevice>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Host, src => src.Host)
                  .Map(dst => dst.Vendor, src => src.Vendor)
                  .Map(dst => dst.Model, src => src.Model)
                  .Map(dst => dst.SoftwareVersion, src => src.SoftwareVersion)
                  .Map(dst => dst.GatewayLevel, src => src.GatewayLevel)
                  .Map(dst => dst.Interfaces, src => src.Interfaces);

            config.NewConfig<CreateOIDTargetCommand, OIDTarget>()
                  .Map(dst => dst.OIDTargetName, src => src.OIDTargetName)
                  .Map(dst => dst.TargetAction, src => src.TargetAction)
                  .Map(dst => dst.Target, src => src.Target)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<UpdateOIDTargetCommand, OIDTarget>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.OIDTargetName, src => src.OIDTargetName)
                  .Map(dst => dst.TargetAction, src => src.TargetAction)
                  .Map(dst => dst.Target, src => src.Target)
                  .Map(dst => dst.OID, src => src.OID);
      }
}
