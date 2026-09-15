namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

public class EdgeDefinition : BaseEntity
{
    /// <summary>
    /// FK to the owning <see cref="WorkflowDefinition"/>.
    /// </summary>
    public Guid WorkflowDefinitionId { get; set; }

    public Guid FromNodeId { get; set; }

    public Guid ToNodeId { get; set; }

    /// <summary>
    /// Logical decision value.
    /// Example: "Huawei", "Extreme", "DEFAULT".
    /// Null means unconditional edge.
    /// </summary>
    public string? Condition { get; set; }

    public int Priority { get; set; }
}