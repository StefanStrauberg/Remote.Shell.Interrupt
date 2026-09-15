namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// Lightweight projection for the paginated list endpoint - avoids shipping every node/edge
/// of every workflow just to render a list.
/// </summary>
public class WorkflowSummaryDTO : IMapWith<WorkflowDefinition>
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public int Version { get; set; }

  public string Status { get; set; } = string.Empty;

  public int NodeCount { get; set; }

  public int EdgeCount { get; set; }

  void IMapWith<WorkflowDefinition>.Mapping(Profile profile)
    => profile.CreateMap<WorkflowDefinition, WorkflowSummaryDTO>()
              .ForMember(dest => dest.NodeCount,
                         opt => opt.MapFrom(src => src.Nodes.Count))
              .ForMember(dest => dest.EdgeCount,
                         opt => opt.MapFrom(src => src.Edges.Count));
}
