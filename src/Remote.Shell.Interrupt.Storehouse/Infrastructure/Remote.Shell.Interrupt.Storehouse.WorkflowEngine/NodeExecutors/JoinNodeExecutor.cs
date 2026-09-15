namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Merge point for parallel branches. Does nothing by itself - it exists so a graph can give
/// several branches (e.g. per-vendor Script nodes) a single downstream node to converge on.
/// </summary>
internal class JoinNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.Join;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
    => Task.FromResult(NodeResult.Ok());
}
