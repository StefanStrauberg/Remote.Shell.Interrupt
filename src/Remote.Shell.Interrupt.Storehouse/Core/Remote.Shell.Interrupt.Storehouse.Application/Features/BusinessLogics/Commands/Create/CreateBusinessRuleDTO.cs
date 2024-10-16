namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Create;

public class CreateBusinessRuleDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfNetworkDevice? Vendor { get; set; }
  public Guid? ParentId { get; set; }
  public Guid? AssignmentId { get; set; }

  void IMapWith<BusinessRule>.Mapping(Profile profile)
  {
    profile.CreateMap<CreateBusinessRuleDTO, BusinessRule>()
           .ForMember(businessRule => businessRule.Id,
                      opt => opt.MapFrom(createBusinessRuleDTO => createBusinessRuleDTO.Id))
           .ForMember(businessRule => businessRule.Name,
                      opt => opt.MapFrom(createBusinessRuleDTO => createBusinessRuleDTO.Name))
           .ForMember(businessRule => businessRule.Vendor,
                      opt => opt.MapFrom(createBusinessRuleDTO => createBusinessRuleDTO.Vendor))
           .ForMember(businessRule => businessRule.ParentId,
                      opt => opt.MapFrom(createBusinessRuleDTO => createBusinessRuleDTO.ParentId))
           .ForMember(businessRule => businessRule.AssignmentId,
                      opt => opt.MapFrom(createBusinessRuleDTO => createBusinessRuleDTO.AssignmentId));
  }
}
