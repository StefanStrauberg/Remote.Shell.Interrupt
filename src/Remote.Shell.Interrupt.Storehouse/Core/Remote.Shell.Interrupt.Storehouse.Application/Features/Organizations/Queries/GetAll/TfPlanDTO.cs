namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public class TfPlanDTO : IMapWith<TfPlanR>
{
  public int Id { get; set; }
  public string NameTfPlan { get; set; } = string.Empty;
  public string DescTfPlan { get; set; } = string.Empty;

  void IMapWith<TfPlanR>.Mapping(Profile profile)
  {
    profile.CreateMap<TfPlanR, TfPlanDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.NameTfPlan,
                      opt => opt.MapFrom(src => src.NameTfPlan.TrimEnd()))
           .ForMember(dest => dest.DescTfPlan,
                      opt => opt.MapFrom(src => src.DescTfPlan.TrimEnd()));
  }
}
