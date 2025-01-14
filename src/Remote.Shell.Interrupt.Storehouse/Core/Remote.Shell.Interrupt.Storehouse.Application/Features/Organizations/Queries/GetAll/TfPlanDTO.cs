namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class TfPlanDTO : IMapWith<TfPlanL>
{
  public string NameTfPlan { get; set; } = string.Empty;
  public string DescTfPlan { get; set; } = string.Empty;

  void IMapWith<TfPlanL>.Mapping(Profile profile)
  {
    profile.CreateMap<TfPlanL, TfPlanDTO>()
           .ForMember(dest => dest.NameTfPlan,
                      opt => opt.MapFrom(src => src.NameTfPlan))
           .ForMember(dest => dest.DescTfPlan,
                      opt => opt.MapFrom(src => src.DescTfPlan));
  }
}
