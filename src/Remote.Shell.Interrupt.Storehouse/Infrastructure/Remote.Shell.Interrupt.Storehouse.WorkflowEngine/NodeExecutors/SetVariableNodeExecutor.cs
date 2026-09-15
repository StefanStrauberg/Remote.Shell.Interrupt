namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Sets a context variable to a literal value from config. <c>Config</c>: "name" (required,
/// the context key to write), "value" (any JSON-compatible value).
/// </summary>
internal class SetVariableNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.SetVariable;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var name = definition.Config.GetValueOrDefault("name")?.ToString();

    if (string.IsNullOrWhiteSpace(name))
      throw new InvalidOperationException("SetVariable node requires config.name");

    var value = definition.Config.GetValueOrDefault("value");
    context.Set(name, value);

    return Task.FromResult(NodeResult.Ok(new() { [name] = value }));
  }
}
