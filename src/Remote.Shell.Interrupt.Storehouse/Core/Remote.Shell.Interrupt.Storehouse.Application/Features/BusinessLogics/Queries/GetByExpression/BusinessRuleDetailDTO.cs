namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;

public class BusinessRuleDetailDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; private set; }
  public DateTime Created { get; set; }
  public DateTime Modified { get; set; }
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; } // Represent what to do
  public bool IsRoot { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<BusinessRule, BusinessRuleDetailDTO>()
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Id,
                      opt => opt.MapFrom(businessRule => businessRule.Id))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Created,
                      opt => opt.MapFrom(businessRule => businessRule.Created))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Modified,
                      opt => opt.MapFrom(businessRule => businessRule.Modified))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Name,
                      opt => opt.MapFrom(businessRule => businessRule.Name))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Condition,
                      opt => opt.MapFrom(businessRule => businessRule.Condition))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.ParentId,
                      opt => opt.MapFrom(businessRule => businessRule.ParentId))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Children,
                      opt => opt.MapFrom(businessRule => businessRule.Children))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.AssignmentId,
                      opt => opt.MapFrom(businessRule => businessRule.AssignmentId))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.IsRoot,
                      opt => opt.MapFrom(businessRule => businessRule.IsRoot));
  }
}
