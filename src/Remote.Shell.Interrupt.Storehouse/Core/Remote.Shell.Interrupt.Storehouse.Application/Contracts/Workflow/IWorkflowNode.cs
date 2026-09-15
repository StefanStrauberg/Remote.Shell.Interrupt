namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;
public interface IWorkflowNode
{
  /// <summary>
  /// The technical node type this executor handles (see <see cref="WorkflowNodeTypes"/>).
  /// </summary>
  string Type { get; }

  Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken);
}