namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Reads a context variable and turns it into a <see cref="NodeResult.Decision"/> that the
/// engine matches against outgoing edge conditions. <c>Config["mode"]</c> selects how:
/// "value" (default) uses the variable's raw string value; "firmware-major" reads the major
/// version number out of a firmware string and buckets it into MODERN (>=10) or LEGACY.
/// </summary>
internal class DecisionNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.Decision;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var variable = definition.Config.GetValueOrDefault("variable")?.ToString()
                   ?? throw new InvalidOperationException("Decision node requires config.variable");

    var mode = definition.Config.GetValueOrDefault("mode")?.ToString() ?? "value";
    var value = context.Get(variable)?.ToString() ?? string.Empty;

    var decision = mode switch
    {
      "value" => value,
      "firmware-major" => GetFirmwareDecision(value),
      _ => throw new InvalidOperationException($"Unknown decision mode '{mode}'.")
    };

    return Task.FromResult(NodeResult.WithDecision(decision));
  }

  static string GetFirmwareDecision(string firmware)
  {
    var majorText = firmware.Split('.', '-', '_')[0];

    if (!int.TryParse(majorText, out var major))
      return "LEGACY";

    return major >= 10 ? "MODERN" : "LEGACY";
  }
}
