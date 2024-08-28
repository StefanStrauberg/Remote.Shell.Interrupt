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
                  .Map(dst => dst.Branch, src => src.Branch)
                  .Map(dst => dst.SequenceNumber, src => src.SequenceNumber)
                  .Map(dst => dst.Assignment, src => src.Assignment);

            config.NewConfig<CreateAssignmentDTO, Assignment>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<CreateBusinessRuleDTO, BusinessRule>()
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.Condition, src => src.Condition)
                  .Map(dst => dst.Branch, src => src.Branch)
                  .Map(dst => dst.Assignment, src => src.Assignment);

            config.NewConfig<UpdateAssignmentDTO, Assignment>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Name, src => src.Name)
                  .Map(dst => dst.TypeOfRequest, src => src.TypeOfRequest)
                  .Map(dst => dst.TargetFieldName, src => src.TargetFieldName)
                  .Map(dst => dst.OID, src => src.OID);

            config.NewConfig<UpdateNetworkDevice, NetworkDevice>()
                  .Map(dst => dst.Id, src => src.Id)
                  .Map(dst => dst.Host, src => src.Host)
                  .Map(dst => dst.Ports, src => src.Ports);
      }
}
