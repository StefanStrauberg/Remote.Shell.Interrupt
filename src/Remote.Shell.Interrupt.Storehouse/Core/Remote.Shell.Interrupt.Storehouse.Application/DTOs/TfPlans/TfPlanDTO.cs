namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.TfPlans;

public class TfPlanDTO : IRegister
{
  public Guid Id { get; set; }
  public int IdTfPlan { get; set; }
  public string NameTfPlan { get; set; } = string.Empty;
  public string DescTfPlan { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<TfPlan, TfPlanDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.IdTfPlan,
               src => src.IdTfPlan)
          .Map(dest => dest.NameTfPlan,
               src => src.NameTfPlan)
          .Map(dest => dest.DescTfPlan,
               src => src.DescTfPlan);
  }
}