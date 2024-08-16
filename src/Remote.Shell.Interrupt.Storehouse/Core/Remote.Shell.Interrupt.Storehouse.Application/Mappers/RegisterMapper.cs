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

            config.NewConfig<CreateAssignmentCommand, Assignment>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<UpdateAssignmentCommand, Assignment>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);
      }
}
