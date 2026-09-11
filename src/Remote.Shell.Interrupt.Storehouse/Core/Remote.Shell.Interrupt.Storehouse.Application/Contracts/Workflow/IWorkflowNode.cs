namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;
public interface IWorkflowNode
{
  Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken);
}