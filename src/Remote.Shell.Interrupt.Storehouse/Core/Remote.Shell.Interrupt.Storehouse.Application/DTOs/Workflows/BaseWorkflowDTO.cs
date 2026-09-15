namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// Shared property holder for the Workflow DTOs. Deliberately does not implement
/// <see cref="IMapWith{T}"/> - each derived DTO registers its own complete AutoMapper map,
/// same reasoning as <c>BaseGateDTO</c>.
/// </summary>
public class BaseWorkflowDTO
{
  public string Name { get; set; } = string.Empty;

  public int Version { get; set; }

  public Guid StartNodeId { get; set; }

  public List<WorkflowNodeDTO> Nodes { get; set; } = [];

  public List<WorkflowEdgeDTO> Edges { get; set; } = [];
}
