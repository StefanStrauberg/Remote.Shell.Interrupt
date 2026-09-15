namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Runs a sandboxed JavaScript transform (via Jint), matching the WorkflowDesignerReact
/// contract: <c>Config["scriptSource"]</c> defines <c>function execute(input, context) {...}</c>.
/// <c>input</c> is resolved from <c>Config["input"]</c> (a context key; empty/absent means
/// "pass the whole context"). A plain return value is written to <c>Config["output"]</c>
/// (defaults to <c>script.&lt;Key or Id&gt;</c>); a NodeResult-shaped return
/// (<c>{success, decision, outputs, error}</c>) is used as-is - <c>outputs</c>' keys are
/// themselves context keys, written directly like every other node's outputs.
/// <c>Config["timeoutMs"]</c> (default 1500, clamped to [50, 30000]) bounds the script's
/// running time; the engine is also constrained on recursion depth and memory so a malformed
/// or runaway script can't hang or crash the host process - scripts are Admin-authored, but
/// still arbitrary code.
/// </summary>
internal class ScriptNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.Script;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var source = definition.Config.GetValueOrDefault("scriptSource")?.ToString()
                 ?? throw new InvalidOperationException("Script node requires config.scriptSource");

    var inputPath = definition.Config.GetValueOrDefault("input")?.ToString() ?? string.Empty;
    var outputPath = definition.Config.GetValueOrDefault("output")?.ToString()
                      ?? $"script.{definition.Key ?? definition.Id.ToString()}";
    var timeoutMs = ClampTimeout(definition.Config.GetValueOrDefault("timeoutMs"));

    var input = string.IsNullOrWhiteSpace(inputPath)
      ? new Dictionary<string, object?>(context.Variables)
      : context.Get(inputPath);

    var logs = new List<string>();
    var engine = new Engine(options => options.LimitRecursion(64)
                                              .TimeoutInterval(TimeSpan.FromMilliseconds(timeoutMs))
                                              .LimitMemory(4_000_000));

    engine.SetValue("console", new ConsoleBridge(logs));

    try
    {
      engine.Execute(source);
      var returnValue = engine.Invoke("execute", input, new Dictionary<string, object?>(context.Variables));

      return Task.FromResult(ToNodeResult(returnValue.ToObject(), outputPath, logs));
    }
    catch (Exception ex)
    {
      return Task.FromResult(NodeResult.Failed($"Script execution failed: {ex.Message}", logs));
    }
  }

  static int ClampTimeout(object? raw)
  {
    var value = raw is null ? 1500 : Convert.ToInt32(raw);
    return Math.Clamp(value, 50, 30000);
  }

  static NodeResult ToNodeResult(object? returnValue, string outputPath, List<string> logs)
  {
    if (returnValue is IDictionary<string, object> obj && IsNodeResultShaped(obj))
    {
      var outputs = obj.TryGetValue("outputs", out var rawOutputs) && rawOutputs is IDictionary<string, object> outputsDict
        ? outputsDict.ToDictionary(kv => kv.Key, kv => (object?)kv.Value)
        : [];

      var success = !(obj.TryGetValue("success", out var rawSuccess) && rawSuccess is false);
      var decision = obj.TryGetValue("decision", out var rawDecision) && rawDecision is not null ? rawDecision.ToString() : null;
      var error = obj.TryGetValue("error", out var rawError) && rawError is not null ? rawError.ToString() : null;

      return new NodeResult
      {
        Success = success,
        Decision = decision,
        Outputs = outputs,
        Error = error,
        Logs = logs
      };
    }

    return new NodeResult
    {
      Outputs = new() { [outputPath] = returnValue },
      Logs = logs
    };
  }

  static bool IsNodeResultShaped(IDictionary<string, object> obj)
    => obj.ContainsKey("outputs") || obj.ContainsKey("decision") || obj.ContainsKey("success") || obj.ContainsKey("error");

  /// <summary>
  /// Exposed to scripts as the global <c>console</c> - captures log/warn/error calls into the
  /// node's <see cref="NodeResult.Logs"/> instead of writing anywhere real.
  /// </summary>
  sealed class ConsoleBridge(List<string> logs)
  {
    public void log(params object?[] args) => logs.Add(Format(args));
    public void warn(params object?[] args) => logs.Add("[warn] " + Format(args));
    public void error(params object?[] args) => logs.Add("[error] " + Format(args));

    static string Format(object?[] args)
      => string.Join(" ", args.Select(a => a?.ToString() ?? "null"));
  }
}
