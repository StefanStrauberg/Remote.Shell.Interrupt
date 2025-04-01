namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;

public class TfPlanDTO : IMapWith<TfPlan>
{
  public string NameTfPlan { get; set; } = string.Empty;
  public string DescTfPlan { get; set; } = string.Empty;

  void IMapWith<TfPlan>.Mapping(Profile profile)
  {
    profile.CreateMap<TfPlan, TfPlanDTO>()
           .ForMember(dest => dest.NameTfPlan,
                      opt => opt.MapFrom(src => src.NameTfPlan))
           .ForMember(dest => dest.DescTfPlan,
                      opt => opt.MapFrom(src => src.DescTfPlan));
  }
}
