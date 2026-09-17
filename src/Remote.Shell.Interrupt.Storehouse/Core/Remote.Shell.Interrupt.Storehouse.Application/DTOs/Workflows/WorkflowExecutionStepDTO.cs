namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class WorkflowExecutionStepDTO : IRegister
{
  public string NodeName { get; set; } = string.Empty;

  public string NodeType { get; set; } = string.Empty;

  public Dictionary<string, object?> Outputs { get; set; } = [];

  public string? Decision { get; set; }

  public List<CandidateEdgeDTO> CandidateEdges { get; set; } = [];

  public string Why { get; set; } = string.Empty;

  public string? NextNodeName { get; set; }

  public List<string> Logs { get; set; } = [];

  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<WorkflowExecutionStep, WorkflowExecutionStepDTO>();
}
