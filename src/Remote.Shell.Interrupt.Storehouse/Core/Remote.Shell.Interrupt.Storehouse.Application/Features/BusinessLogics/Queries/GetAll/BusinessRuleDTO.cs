namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;

public class BusinessRuleDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; private set; }
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; } // Represent what to do
  public bool IsRoot { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<BusinessRule, BusinessRuleDTO>()
           .ForMember(businessRuleDTO => businessRuleDTO.Id,
                      opt => opt.MapFrom(businessRule => businessRule.Id))
           .ForMember(businessRuleDTO => businessRuleDTO.Name,
                      opt => opt.MapFrom(businessRule => businessRule.Name))
           .ForMember(businessRuleDTO => businessRuleDTO.Condition,
                      opt => opt.MapFrom(businessRule => businessRule.Condition))
           .ForMember(businessRuleDTO => businessRuleDTO.ParentId,
                      opt => opt.MapFrom(businessRule => businessRule.ParentId))
           .ForMember(businessRuleDTO => businessRuleDTO.Children,
                      opt => opt.MapFrom(businessRule => businessRule.Children.Select(x => x.Id)))
           .ForMember(businessRuleDTO => businessRuleDTO.AssignmentId,
                      opt => opt.MapFrom(businessRule => businessRule.AssignmentId))
           .ForMember(businessRuleDTO => businessRuleDTO.IsRoot,
                      opt => opt.MapFrom(businessRule => businessRule.IsRoot));
  }
}
