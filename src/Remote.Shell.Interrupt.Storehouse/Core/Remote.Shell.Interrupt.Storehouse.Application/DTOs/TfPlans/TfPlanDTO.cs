namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.TfPlans;

public class TfPlanDTO : IMapWith<TfPlan>
{
  public Guid Id { get; set; }
  public int IdTfPlan { get; set; }
  public string NameTfPlan { get; set; } = string.Empty;
  public string DescTfPlan { get; set; } = string.Empty;

  void IMapWith<TfPlan>.Mapping(Profile profile)
  {
    profile.CreateMap<TfPlan, TfPlanDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.IdTfPlan,
                      opt => opt.MapFrom(src => src.IdTfPlan))
           .ForMember(dest => dest.NameTfPlan,
                      opt => opt.MapFrom(src => src.NameTfPlan))
           .ForMember(dest => dest.DescTfPlan,
                      opt => opt.MapFrom(src => src.DescTfPlan));
  }
}