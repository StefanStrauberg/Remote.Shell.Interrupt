namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class NodeDefinition : BaseEntity
{
    /// <summary>
    /// FK to the owning <see cref="WorkflowDefinition"/>.
    /// </summary>
    public Guid WorkflowDefinitionId { get; set; }

    /// <summary>
    /// Technical node type.
    /// Example: Start, SnmpGet, Script, Decision, End.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name for UI.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional stable logical key.
    /// Example: "get-sysdescr".
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Node-specific configuration.
    /// </summary>
    public Dictionary<string, object?> Config { get; set; } = [];

    public double PositionX { get; set; }

    public double PositionY { get; set; }
}