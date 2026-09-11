namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class WorkflowDefinition : BaseEntity
{

    public string Name { get; init; } = string.Empty;

    public int Version { get; init; }

    public Guid StartNodeId { get; init; }

    public List<NodeDefinition> Nodes { get; init; } = [];

    public List<EdgeDefinition> Edges { get; init; } = [];
  }