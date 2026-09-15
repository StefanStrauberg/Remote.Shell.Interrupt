namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

/// <summary>
/// The outcome of running a <see cref="WorkflowDefinition"/> to completion or failure.
/// Not persisted - built fresh on every execution request.
/// </summary>
public class WorkflowExecutionResult
{
  public bool Success { get; init; } = true;

  public string? Error { get; init; }

  public List<WorkflowExecutionStep> Steps { get; init; } = [];

  public IReadOnlyDictionary<string, object?> FinalVariables { get; init; } =
    new Dictionary<string, object?>();
}
