namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;

/// <summary>
/// The known <see cref="NodeDefinition.Type"/> values. Shared between validation (here)
/// and the concrete <see cref="IWorkflowNode"/> executors (Infrastructure) so both sides
/// agree on the same set of technical node types without either depending on the other's
/// implementation details.
/// </summary>
public static class WorkflowNodeTypes
{
  public const string Start = "Start";
  public const string End = "End";
  public const string Decision = "Decision";
  public const string Join = "Join";
  public const string SetVariable = "SetVariable";
  public const string SnmpGet = "SnmpGet";
  public const string SnmpWalk = "SnmpWalk";
  public const string Script = "Script";

  public static readonly IReadOnlySet<string> All = new HashSet<string>(
    [Start, End, Decision, Join, SetVariable, SnmpGet, SnmpWalk, Script],
    StringComparer.OrdinalIgnoreCase);
}
