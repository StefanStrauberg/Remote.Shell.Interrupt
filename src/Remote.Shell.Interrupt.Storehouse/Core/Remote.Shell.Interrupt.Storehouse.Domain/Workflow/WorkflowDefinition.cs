namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class WorkflowDefinition : BaseEntity
{

    public string Name { get; set; } = string.Empty;

    public int Version { get; set; }

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;

    public Guid StartNodeId { get; set; }

    public List<NodeDefinition> Nodes { get; set; } = [];

    public List<EdgeDefinition> Edges { get; set; } = [];
  }