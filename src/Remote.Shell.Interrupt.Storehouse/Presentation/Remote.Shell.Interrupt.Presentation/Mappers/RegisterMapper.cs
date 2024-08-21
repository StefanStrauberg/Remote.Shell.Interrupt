namespace Remote.Shell.Interrupt.Storehouse.Presentation.Mappers;

public class RegisterMapper : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
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

    config.NewConfig<CreateBusinessRuleCommand, BusinessRule>()
          .Map(dst => dst.Name, src => src.Name)
          .Map(dst => dst.Condition, src => src.Condition)
          .Map(dst => dst.Branch, src => src.Branch)
          .Map(dst => dst.SequenceNumber, src => src.SequenceNumber)
          .Map(dst => dst.Assignment, src => src.Assignment);
  }
}
