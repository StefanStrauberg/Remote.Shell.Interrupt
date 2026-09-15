namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Walks an OID subtree on the device the workflow is running against and stores the
/// resulting values (as a string array) in the context. <c>Config</c>: "oid" (required),
/// "output" (required, context key to write), "toHex" (optional bool), "repetitions"
/// (optional int).
/// </summary>
internal class SnmpWalkNodeExecutor(ISNMPCommandExecutor snmp) : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.SnmpWalk;

  public async Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var oid = definition.Config.GetValueOrDefault("oid")?.ToString()
              ?? throw new InvalidOperationException("SnmpWalk node requires config.oid");

    var output = definition.Config.GetValueOrDefault("output")?.ToString()
                 ?? throw new InvalidOperationException("SnmpWalk node requires config.output");

    var toHex = definition.Config.GetValueOrDefault("toHex") is true;

    var repetitions = definition.Config.GetValueOrDefault("repetitions") switch
    {
      null => 20,
      var value => Convert.ToInt32(value)
    };

    var responses = await snmp.WalkCommand(context.Host, context.Community, oid, cancellationToken, toHex, repetitions);

    return NodeResult.Ok(new()
    {
      [output] = responses.Select(r => r.Data).ToArray()
    });
  }
}
