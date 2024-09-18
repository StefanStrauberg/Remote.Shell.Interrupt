namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;

public class BusinessRuleDetailDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; private set; }
  public DateTime Created { get; set; }
  public DateTime Modified { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfNetworkDevice? Vendor { get; set; }
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; }
  public bool IsRoot { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<BusinessRule, BusinessRuleDetailDTO>()
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Id,
                      opt => opt.MapFrom(businessRule => businessRule.Id))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Created,
                      opt => opt.MapFrom(businessRule => businessRule.CreatedAt))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Modified,
                      opt => opt.MapFrom(businessRule => businessRule.UpdatedAt))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Name,
                      opt => opt.MapFrom(businessRule => businessRule.Name))
           .ForMember(businessRuleDetailDTO => businessRuleDetailDTO.Vendor,
                      opt => opt.MapFrom(businessRule => businessRule.Vendor))
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
