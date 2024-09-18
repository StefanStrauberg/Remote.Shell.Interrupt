namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

public class UpdateBusinessRuleDTO : IMapWith<BusinessRule>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfNetworkDevice? Vendor { get; set; }
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
           .ForMember(businessRule => businessRule.Vendor,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.Vendor))
           .ForMember(businessRule => businessRule.ParentId,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.ParentId))
           .ForMember(businessRule => businessRule.Children,
                      opt => opt.Ignore()) // Здесь игнорируем прямое маппирование
           .ForMember(businessRule => businessRule.AssignmentId,
                      opt => opt.MapFrom(updateBusinessRuleDTO => updateBusinessRuleDTO.AssignmentId));
  }
}
