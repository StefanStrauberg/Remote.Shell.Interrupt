namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

/// <summary>
/// A single step of a workflow run: which node executed, what it produced,
/// and why the engine chose the next edge. A structured equivalent of the
/// step-by-step trace a human would want when diagnosing a specific device/vendor path.
/// </summary>
public class WorkflowExecutionStep
{
  public string NodeName { get; init; } = string.Empty;

  public string NodeType { get; init; } = string.Empty;

  public Dictionary<string, object?> Outputs { get; init; } = [];

  public string? Decision { get; init; }

  public List<CandidateEdge> CandidateEdges { get; init; } = [];

  public string Why { get; init; } = string.Empty;

  public string? NextNodeName { get; init; }

  public List<string> Logs { get; init; } = [];
}

/// <summary>
/// One outgoing edge considered while resolving the next node, for the execution trace.
/// </summary>
public class CandidateEdge
{
  public string? Condition { get; init; }

  public int Priority { get; init; }
}
