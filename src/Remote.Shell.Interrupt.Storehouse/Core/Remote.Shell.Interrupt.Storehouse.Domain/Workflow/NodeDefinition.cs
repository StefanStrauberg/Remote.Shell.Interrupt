namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class NodeDefinition : BaseEntity
{
    /// <summary>
    /// Technical node type.
    /// Example: Start, SnmpGet, Script, Decision, End.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Human-readable name for UI.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Optional stable logical key.
    /// Example: "get-sysdescr".
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Node-specific configuration.
    /// </summary>
    public Dictionary<string, object?> Config { get; init; } = [];

    public double PositionX { get; init; }

    public double PositionY { get; init; }
}