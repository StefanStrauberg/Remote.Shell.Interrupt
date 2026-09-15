namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine;

/// <summary>
/// Runs a <see cref="WorkflowDefinition"/> from its start node to an End node, routing
/// between nodes via <see cref="EdgeDefinition"/> priority/condition matching. Has no
/// vendor-specific knowledge - ported from the WorkflowExample prototype's engine, which
/// already proved this edge-resolution algorithm against Huawei/Juniper/DEFAULT scenarios.
/// </summary>
internal class WorkflowEngine(IWorkflowNodeResolver resolver) : IWorkflowEngine
{
  // Edge routing here is by priority/condition matching, not a DAG the designer enforces to be
  // acyclic - nothing stops a saved graph (crafted by hand via CreateWorkflow's JSON import, or
  // just a mistake in the visual designer) from looping a Decision node back on itself. Without
  // a step cap, executing that graph would run this while(true) forever: cancellationToken is
  // only the caller's HTTP RequestAborted, which never fires on its own unless the client
  // disconnects. This bounds a run to something far past any legitimate graph's node count
  // (the seeded network-device-discovery workflow, the largest shipped with this app, is under
  // 20 nodes) while still catching a genuine cycle quickly.
  const int MaxSteps = 1_000;

  public async Task<WorkflowExecutionResult> ExecuteAsync(WorkflowDefinition workflow,
                                                          WorkflowContext context,
                                                          CancellationToken cancellationToken)
  {
    var steps = new List<WorkflowExecutionStep>();
    var currentNodeId = workflow.StartNodeId;
    var stepCount = 0;

    while (true)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (++stepCount > MaxSteps)
        return Failed(steps, $"Workflow '{workflow.Name}' exceeded the maximum of {MaxSteps} node executions - likely a cycle in the graph's edges.");

      var node = workflow.Nodes.SingleOrDefault(x => x.Id == currentNodeId);

      if (node is null)
        return Failed(steps, $"Node '{currentNodeId}' does not exist in workflow '{workflow.Name}'.");

      context.CurrentNodeId = node.Id;

      var executor = resolver.Resolve(node.Type);
      NodeResult result;

      try
      {
        result = await executor.ExecuteAsync(node, context, cancellationToken);
      }
      catch (Exception ex) when (ex is not OperationCanceledException)
      {
        // Only ScriptNodeExecutor is internally exception-safe (a malformed script is
        // expected, routine input). Every other executor throws plain C# exceptions for
        // misconfiguration/invalid input (e.g. a missing config key, an unparsable value) -
        // catching here means every node failure becomes part of the trace as Success:false,
        // not an uncaught exception that skips the trace and surfaces as a raw 500.
        return Failed(steps, $"Node '{node.Name}' threw {ex.GetType().Name}: {ex.Message}");
      }

      if (!result.Success)
        return Failed(steps, $"Node '{node.Name}' failed: {result.Error}");

      foreach (var (key, value) in result.Outputs)
        context.Set(key, value);

      if (string.Equals(node.Type, WorkflowNodeTypes.End, StringComparison.OrdinalIgnoreCase))
      {
        steps.Add(new WorkflowExecutionStep
        {
          NodeName = node.Name,
          NodeType = node.Type,
          Outputs = result.Outputs,
          Decision = result.Decision,
          Why = "End node reached.",
          Logs = result.Logs
        });

        return new WorkflowExecutionResult
        {
          Success = true,
          Steps = steps,
          FinalVariables = context.Variables
        };
      }

      var (next, candidates, why) = ResolveNextEdge(workflow, node, result.Decision);

      if (next is null)
      {
        steps.Add(new WorkflowExecutionStep
        {
          NodeName = node.Name,
          NodeType = node.Type,
          Outputs = result.Outputs,
          Decision = result.Decision,
          CandidateEdges = candidates,
          Why = why,
          Logs = result.Logs
        });

        return Failed(steps, why);
      }

      var nextNode = workflow.Nodes.Single(x => x.Id == next.ToNodeId);

      steps.Add(new WorkflowExecutionStep
      {
        NodeName = node.Name,
        NodeType = node.Type,
        Outputs = result.Outputs,
        Decision = result.Decision,
        CandidateEdges = candidates,
        Why = why,
        NextNodeName = nextNode.Name,
        Logs = result.Logs
      });

      currentNodeId = next.ToNodeId;
    }
  }

  static WorkflowExecutionResult Failed(List<WorkflowExecutionStep> steps, string error)
    => new()
    {
      Success = false,
      Error = error,
      Steps = steps
    };

  static (EdgeDefinition? Edge, List<CandidateEdge> Candidates, string Why) ResolveNextEdge(WorkflowDefinition workflow,
                                                                                            NodeDefinition node,
                                                                                            string? decision)
  {
    var edges = workflow.Edges
                        .Where(x => x.FromNodeId == node.Id)
                        .OrderBy(x => x.Priority)
                        .ToList();

    var candidates = edges.Select(e => new CandidateEdge { Condition = e.Condition, Priority = e.Priority })
                          .ToList();

    if (edges.Count == 0)
      return (null, candidates, $"Node '{node.Name}' has no outgoing edges.");

    if (decision is not null)
    {
      var exact = edges.FirstOrDefault(x =>
        string.Equals(x.Condition, decision, StringComparison.OrdinalIgnoreCase));

      if (exact is not null)
        return (exact, candidates, $"Exact edge condition '{exact.Condition}' matches decision '{decision}'.");
    }

    var fallback = edges.FirstOrDefault(x =>
                       string.Equals(x.Condition, "DEFAULT", StringComparison.OrdinalIgnoreCase))
                   ?? edges.FirstOrDefault(x => x.Condition is null);

    if (fallback is not null)
      return (fallback, candidates, $"No exact match; fallback edge {DescribeCondition(fallback.Condition)} selected.");

    return (null, candidates, $"Cannot resolve next edge for node '{node.Name}', decision='{decision ?? "<null>"}'.");
  }

  static string DescribeCondition(string? condition) =>
    condition is null ? "<unconditional>" : $"'{condition}'";
}
