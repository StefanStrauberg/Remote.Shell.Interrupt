namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

public class EdgeDefinition : BaseEntity
{

    public Guid FromNodeId { get; init; }

    public Guid ToNodeId { get; init; }

    /// <summary>
    /// Logical decision value.
    /// Example: "Huawei", "Extreme", "DEFAULT".
    /// Null means unconditional edge.
    /// </summary>
    public string? Condition { get; init; }

    public int Priority { get; init; }
}