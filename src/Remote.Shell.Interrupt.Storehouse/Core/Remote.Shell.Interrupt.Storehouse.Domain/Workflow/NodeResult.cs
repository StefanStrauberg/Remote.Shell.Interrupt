namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class NodeResult
{
    public bool Success { get; init; } = true;

    public string? Decision { get; init; }

    public Dictionary<string, object?> Outputs { get; init; } = [];

    public string? Error { get; init; }

    public static NodeResult Ok()
    {
        return new NodeResult();
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
}