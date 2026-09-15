namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// A single directed edge of a workflow graph, connecting two <see cref="WorkflowNodeDTO"/>
/// entries of the same payload by their caller-assigned <see cref="WorkflowNodeDTO.Id"/>.
/// Like <see cref="WorkflowNodeDTO.Id"/>, <see cref="Id"/> is caller-assigned so a client
/// (e.g. an undo stack or an inspector panel) can keep a stable reference to a specific edge
/// across a round-trip through the API.
/// </summary>
public class WorkflowEdgeDTO : IMapWith<EdgeDefinition>
{
  public Guid Id { get; set; }

  public Guid FromNodeId { get; set; }

  public Guid ToNodeId { get; set; }

  public string? Condition { get; set; }

  public int Priority { get; set; }

  void IMapWith<EdgeDefinition>.Mapping(Profile profile)
    => profile.CreateMap<EdgeDefinition, WorkflowEdgeDTO>()
              .ReverseMap();
}
