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
    // Every Script node receives the whole accumulated context (raw SNMP walks, ports, etc.),
    // not just its declared input - so the memory ceiling has to cover a real device's full
    // discovery data (an interface/ARP/MAC/VLAN table dump easily reaches tens of megabytes on
    // production hardware), not just the toy graphs used in tests. 64 MB still bounds a
    // malformed or runaway script well below what would threaten the host process.
    var engine = new Engine(options =>
    {
      options.LimitRecursion(64)
             .TimeoutInterval(TimeSpan.FromMilliseconds(timeoutMs))
             .LimitMemory(64_000_000);

      // Interop must stay enabled - the script's `input`/`context` parameters and the
      // `console` bridge are all wrapped CLR objects - but every option that would let a
      // script walk from those wrapped objects into CLR reflection is pinned off explicitly
      // rather than left to the library's current defaults, so a future Jint upgrade can't
      // silently reopen this: AllowGetType blocks `context.GetType()` (the classic route from
      // an ObjectWrapper to System.Type -> System.Reflection -> arbitrary CLR invocation),
      // AllowSystemReflection blocks wrapping anything from System.Reflection even if reached
      // another way, and an empty AllowedAssemblies means no CLR type is importable by name
      // (no `System.IO.File`-style access) even though nothing here registers any.
      options.Interop.AllowGetType = false;
      options.Interop.AllowSystemReflection = false;
      options.Interop.AllowedAssemblies = [];
      // Keep CLR resolution failures terse so a probing script can't enumerate the host's
      // wrapped types/members through the exception text.
      options.Interop.ExposeDetailedResolutionErrors = false;
    });

    engine.SetValue("console", new ConsoleBridge(logs));

    try
    {
      engine.Execute(WorkflowScriptPrelude.Source);
      engine.Execute(source);
      var returnValue = engine.Invoke("execute", DeepNormalize(input), new Dictionary<string, object?>(context.Variables));

      return Task.FromResult(ToNodeResult(DeepNormalize(returnValue.ToObject()), outputPath, logs));
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
    if (returnValue is Dictionary<string, object?> obj && IsNodeResultShaped(obj))
    {
      var outputs = obj.TryGetValue("outputs", out var rawOutputs) && rawOutputs is Dictionary<string, object?> outputsDict
        ? outputsDict
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

  static bool IsNodeResultShaped(Dictionary<string, object?> obj)
    => obj.ContainsKey("outputs") || obj.ContainsKey("decision") || obj.ContainsKey("success") || obj.ContainsKey("error");

  /// <summary>
  /// Recursively converts a Jint <c>ToObject()</c> graph (or anything being fed back into
  /// Jint as an argument) into plain, genuinely mutable <see cref="Dictionary{TKey,TValue}"/>/
  /// <see cref="List{T}"/> structures. Jint's own conversion produces fixed-size CLR arrays
  /// (<c>object[]</c>) for JS arrays; feeding one of those back into a later script and
  /// calling <c>.push()</c> on it throws ("Cannot resize a fixed-size CLR array") - Script
  /// nodes routinely do exactly that (e.g. accumulating a port's VLANs across several nodes).
  /// </summary>
  internal static object? DeepNormalize(object? value) => value switch
  {
    null => null,
    string s => s,
    IDictionary<string, object> dict => dict.ToDictionary(kv => kv.Key, kv => DeepNormalize(kv.Value)),
    System.Collections.IEnumerable enumerable => enumerable.Cast<object?>().Select(DeepNormalize).ToList(),
    _ => value
  };

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
