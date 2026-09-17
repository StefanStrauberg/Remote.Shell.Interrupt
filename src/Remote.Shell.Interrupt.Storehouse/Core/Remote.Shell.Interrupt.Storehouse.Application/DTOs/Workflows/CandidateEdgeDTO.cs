namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class CandidateEdgeDTO : IRegister
{
  public string? Condition { get; set; }

  public int Priority { get; set; }

  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<CandidateEdge, CandidateEdgeDTO>();
}
