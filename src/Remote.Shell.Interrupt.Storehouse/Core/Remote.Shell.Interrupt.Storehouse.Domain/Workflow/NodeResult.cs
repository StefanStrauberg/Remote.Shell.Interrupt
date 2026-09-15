namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class NodeResult
{
    public bool Success { get; init; } = true;

    public string? Decision { get; init; }

    public Dictionary<string, object?> Outputs { get; init; } = [];

    public string? Error { get; init; }

    /// <summary>
    /// console.log/warn/error output captured while the node ran. Only <c>Script</c> nodes
    /// populate this today.
    /// </summary>
    public List<string> Logs { get; init; } = [];

    public static NodeResult Ok()
    {
        return new NodeResult();
    }

    public static NodeResult Ok(Dictionary<string, object?> outputs)
    {
        return new NodeResult
        {
            Outputs = outputs
        };
    }

    public static NodeResult WithDecision(
        string decision)
    {
        return new NodeResult
        {
            Decision = decision
        };
    }

    public static NodeResult Failed(
        string error)
    {
        return new NodeResult
        {
            Success = false,
            Error = error
        };
    }

    public static NodeResult Failed(
        string error,
        List<string>? logs)
    {
        return new NodeResult
        {
            Success = false,
            Error = error,
            Logs = logs ?? []
        };
    }
}