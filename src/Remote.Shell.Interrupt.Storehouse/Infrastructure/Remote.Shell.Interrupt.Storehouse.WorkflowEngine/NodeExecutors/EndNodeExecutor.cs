namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

internal class EndNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.End;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
    => Task.FromResult(NodeResult.Ok());
}
