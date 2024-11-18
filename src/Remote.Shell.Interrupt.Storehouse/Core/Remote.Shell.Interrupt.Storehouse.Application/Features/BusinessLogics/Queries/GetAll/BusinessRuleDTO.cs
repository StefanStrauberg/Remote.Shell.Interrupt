namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;

public class BusinessRuleDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; private set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfNetworkDevice? Vendor { get; set; }
  public Guid? ParentId { get; set; }
  public List<BusinessRuleDTO> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; }
  public bool IsRoot { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<BusinessRule, BusinessRuleDTO>()
           .ForMember(businessRuleDTO => businessRuleDTO.Id,
                      opt => opt.MapFrom(businessRule => businessRule.Id))
           .ForMember(businessRuleDTO => businessRuleDTO.Name,
                      opt => opt.MapFrom(businessRule => businessRule.Name))
           .ForMember(businessRuleDTO => businessRuleDTO.Vendor,
                      opt => opt.MapFrom(businessRule => businessRule.Vendor))
           .ForMember(businessRuleDTO => businessRuleDTO.ParentId,
                      opt => opt.MapFrom(businessRule => businessRule.ParentId))
           .ForMember(businessRuleDTO => businessRuleDTO.Children,
                      opt => opt.MapFrom(businessRule => businessRule.Children))
           .ForMember(businessRuleDTO => businessRuleDTO.AssignmentId,
                      opt => opt.MapFrom(businessRule => businessRule.AssignmentId))
           .ForMember(businessRuleDTO => businessRuleDTO.IsRoot,
                      opt => opt.MapFrom(businessRule => businessRule.IsRoot));
  }
}
