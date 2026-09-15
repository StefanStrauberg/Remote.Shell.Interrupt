namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// Polls a single OID on the device the workflow is running against and stores the result
/// in the context. <c>Config</c>: "oid" (required), "output" (required, context key to
/// write), "toHex" (optional bool).
/// </summary>
internal class SnmpGetNodeExecutor(ISNMPCommandExecutor snmp) : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.SnmpGet;

  public async Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var oid = definition.Config.GetValueOrDefault("oid")?.ToString()
              ?? throw new InvalidOperationException("SnmpGet node requires config.oid");

    var output = definition.Config.GetValueOrDefault("output")?.ToString()
                 ?? throw new InvalidOperationException("SnmpGet node requires config.output");

    var toHex = definition.Config.GetValueOrDefault("toHex") is true;

    var response = await snmp.GetCommand(context.Host, context.Community, oid, cancellationToken, toHex);

    return NodeResult.Ok(new()
    {
      [output] = response.Data
    });
  }
}
