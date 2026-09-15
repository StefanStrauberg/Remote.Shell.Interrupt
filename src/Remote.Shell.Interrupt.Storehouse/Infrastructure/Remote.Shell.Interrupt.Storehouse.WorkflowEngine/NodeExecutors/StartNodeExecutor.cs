namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

internal class StartNodeExecutor : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.Start;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
    => Task.FromResult(NodeResult.Ok());
}
