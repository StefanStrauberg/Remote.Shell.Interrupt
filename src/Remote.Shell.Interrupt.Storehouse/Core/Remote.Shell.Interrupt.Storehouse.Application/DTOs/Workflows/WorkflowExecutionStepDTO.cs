namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class WorkflowExecutionStepDTO : IMapWith<WorkflowExecutionStep>
{
  public string NodeName { get; set; } = string.Empty;

  public string NodeType { get; set; } = string.Empty;

  public Dictionary<string, object?> Outputs { get; set; } = [];

  public string? Decision { get; set; }

  public List<CandidateEdgeDTO> CandidateEdges { get; set; } = [];

  public string Why { get; set; } = string.Empty;

  public string? NextNodeName { get; set; }

  public List<string> Logs { get; set; } = [];

  void IMapWith<WorkflowExecutionStep>.Mapping(Profile profile)
    => profile.CreateMap<WorkflowExecutionStep, WorkflowExecutionStepDTO>();
}
