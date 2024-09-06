namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

public class UpdateBusinessRuleDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Condition { get; set; } = string.Empty;
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<UpdateBusinessRuleDTO, BusinessRule>()
           .ForMember(businessRule => businessRule.Id,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.Id))
           .ForMember(businessRule => businessRule.Name,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.Name))
           .ForMember(businessRule => businessRule.Condition,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.Condition))
           .ForMember(businessRule => businessRule.ParentId,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.ParentId))
           .ForMember(businessRule => businessRule.Children,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.Children))
           .ForMember(businessRule => businessRule.AssignmentId,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.AssignmentId));
  }
}
