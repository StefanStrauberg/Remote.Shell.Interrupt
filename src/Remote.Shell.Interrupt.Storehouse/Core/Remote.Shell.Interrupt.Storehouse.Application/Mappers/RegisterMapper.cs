namespace Remote.Shell.Interrupt.Storehouse.Application.Mappers;

public class RegisterMapper : IRegister
{
      public void Register(TypeAdapterConfig config)
      {
            config.NewConfig<Assignment, AssignmentDTO>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<BusinessRule, BusinessRuleDTO>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.Condition, src => src.Condition)
                  .Map(dst => dst.ParentId, src => src.ParentId)
                  .Map(dst => dst.Children, src => src.Children)
                  .Map(dst => dst.AssignmentId, src => src.AssignmentId);

            config.NewConfig<NetworkDevice, NetworkDeviceDTO>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.GeneralInformation, src => src.GeneralInformation)
                  .Map(dst => dst.Ports, src => src.Ports);

            config.NewConfig<CreateAssignmentDTO, Assignment>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<CreateBusinessRuleDTO, BusinessRule>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.Condition, src => src.Condition)
                  .Map(dst => dst.ParentId, src => src.ParentId)
                  .Map(dst => dst.AssignmentId, src => src.AssignmentId);

            config.NewConfig<UpdateAssignmentDTO, Assignment>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<UpdateBusinessRuleDTO, BusinessRule>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.Condition, src => src.Condition)
                  .Map(dst => dst.AssignmentId, src => src.AssignmentId);
      }
}
