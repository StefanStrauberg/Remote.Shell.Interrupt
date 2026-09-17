namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// Lightweight projection for the paginated list endpoint - avoids shipping every node/edge
/// of every workflow just to render a list.
/// </summary>
public class WorkflowSummaryDTO : IRegister
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public int Version { get; set; }

  public string Status { get; set; } = string.Empty;

  public int NodeCount { get; set; }

  public int EdgeCount { get; set; }

  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<WorkflowDefinition, WorkflowSummaryDTO>()
             .Map(dest => dest.NodeCount,
                  src => src.Nodes.Count)
             .Map(dest => dest.EdgeCount,
                  src => src.Edges.Count);
}
