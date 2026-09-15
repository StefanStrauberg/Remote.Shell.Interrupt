namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine;

internal class WorkflowNodeResolver(IEnumerable<IWorkflowNode> executors) : IWorkflowNodeResolver
{
  readonly Dictionary<string, IWorkflowNode> _executors = executors.ToDictionary(x => x.Type,
                                                                                 StringComparer.OrdinalIgnoreCase);

  public IWorkflowNode Resolve(string type)
  {
    if (_executors.TryGetValue(type, out var executor))
      return executor;

    throw new InvalidOperationException($"Executor for node type '{type}' is not registered.");
  }
}
