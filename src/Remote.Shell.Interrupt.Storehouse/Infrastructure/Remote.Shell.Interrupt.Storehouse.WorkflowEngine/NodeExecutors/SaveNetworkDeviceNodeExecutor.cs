namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

/// <summary>
/// The engine's one node type that writes to the database: builds a
/// <see cref="NetworkDevice"/> (with its full Ports/VLANs/MAC/ARP/NetworkTable/AggregatedPorts
/// graph) from context variables populated by earlier nodes, and inserts it. Reads:
/// <list type="bullet">
/// <item><c>input.vendor</c> - required, parsed as <see cref="TypeOfNetworkDevice"/>.</item>
/// <item><c>device.generalInformation</c>, <c>device.name</c> - optional strings.</item>
/// <item><c>network.ports</c> - required array of objects shaped
/// <c>{number, name, type, speed, status, mac, description, vlans:[{tag,name}],
/// macTable:[mac], arp:[{mac,ip}], network:[{ip,mask}], aggregatedPortNumbers:[number]}</c>;
/// <c>type</c>/<c>status</c> are the raw numeric SNMP values (parsed via
/// <c>Enum.Parse&lt;PortType&gt;</c>/<c>Enum.Parse&lt;PortStatus&gt;</c>), and
/// <c>aggregatedPortNumbers</c> on a port makes it the parent of those other ports.</item>
/// </list>
/// The device's own <c>Host</c> comes from <see cref="WorkflowContext.Host"/>, not context -
/// it's already known for the whole run.
/// </summary>
internal class SaveNetworkDeviceNodeExecutor(INetDevUnitOfWork netDevUnitOfWork) : IWorkflowNode
{
  public string Type => WorkflowNodeTypes.SaveNetworkDevice;

  public Task<NodeResult> ExecuteAsync(NodeDefinition definition, WorkflowContext context, CancellationToken cancellationToken)
  {
    var vendorRaw = context.Get("input.vendor")?.ToString()
                    ?? throw new InvalidOperationException("SaveNetworkDevice requires context 'input.vendor' to be set.");

    if (!Enum.TryParse<TypeOfNetworkDevice>(vendorRaw, ignoreCase: true, out var vendor))
      throw new InvalidOperationException($"Unknown network device type '{vendorRaw}'.");

    var networkDevice = new NetworkDevice
    {
      Id = Guid.NewGuid(),
      Host = ConvertStringIPAddressToLong.Handle(context.Host),
      TypeOfNetworkDevice = vendor,
      GeneralInformation = context.Get("device.generalInformation")?.ToString() ?? string.Empty,
      NetworkDeviceName = context.Get("device.name")?.ToString() ?? string.Empty
    };

    var rawPorts = AsList(context.Get("network.ports")).OfType<IDictionary<string, object?>>().ToList();
    var portsByNumber = new Dictionary<int, Port>();
    var vlansByTag = new Dictionary<int, VLAN>();

    foreach (var portData in rawPorts)
    {
      var port = BuildPort(portData, networkDevice.Id, vlansByTag);
      portsByNumber[port.InterfaceNumber] = port;
    }

    foreach (var portData in rawPorts)
    {
      if (!portsByNumber.TryGetValue(ToInt(Value(portData, "number")), out var parent))
        continue;

      foreach (var childNumber in ToIntList(Value(portData, "aggregatedPortNumbers")))
      {
        if (portsByNumber.TryGetValue(childNumber, out var child) && !parent.AggregatedPorts.Contains(child))
        {
          parent.AggregatedPorts.Add(child);
          child.ParentId = parent.Id;
        }
      }
    }

    networkDevice.PortsOfNetworkDevice = [.. portsByNumber.Values];

    netDevUnitOfWork.NetworkDevices.InsertOne(networkDevice);
    netDevUnitOfWork.Complete();

    return Task.FromResult(NodeResult.Ok(new() { ["device.id"] = networkDevice.Id }));
  }

  static Port BuildPort(IDictionary<string, object?> data, Guid networkDeviceId, Dictionary<int, VLAN> vlansByTag)
  {
    var port = new Port
    {
      Id = Guid.NewGuid(),
      NetworkDeviceId = networkDeviceId,
      InterfaceNumber = ToInt(Value(data, "number")),
      InterfaceName = Value(data, "name")?.ToString() ?? string.Empty,
      InterfaceType = Enum.Parse<PortType>(Value(data, "type")?.ToString() ?? "0"),
      InterfaceStatus = Enum.Parse<PortStatus>(Value(data, "status")?.ToString() ?? "0"),
      InterfaceSpeed = ToLong(Value(data, "speed")),
      MACAddress = Value(data, "mac")?.ToString() ?? string.Empty,
      Description = Value(data, "description")?.ToString() ?? string.Empty
    };

    foreach (var vlan in AsDictList(Value(data, "vlans")))
    {
      var tag = ToInt(Value(vlan, "tag"));

      // One VLAN row per unique tag, shared across every port that carries it - not a
      // fresh row per port-VLAN membership.
      if (!vlansByTag.TryGetValue(tag, out var vlanEntity))
      {
        vlanEntity = new VLAN { Id = Guid.NewGuid(), VLANTag = tag, VLANName = Value(vlan, "name")?.ToString() ?? string.Empty };
        vlansByTag[tag] = vlanEntity;
      }

      port.VLANs.Add(vlanEntity);
    }

    foreach (var mac in AsList(Value(data, "macTable")))
      port.MACTable.Add(new MACEntity { Id = Guid.NewGuid(), MACAddress = mac?.ToString() ?? string.Empty, PortId = port.Id });

    foreach (var arp in AsDictList(Value(data, "arp")))
      port.ARPTableOfInterface.Add(new ARPEntity
      {
        Id = Guid.NewGuid(),
        MAC = Value(arp, "mac")?.ToString() ?? string.Empty,
        IPAddress = Value(arp, "ip")?.ToString() ?? string.Empty,
        PortId = port.Id
      });

    foreach (var net in AsDictList(Value(data, "network")))
    {
      var entity = new TerminatedNetworkEntity { Id = Guid.NewGuid(), PortId = port.Id };
      entity.SetAddressAndMask(Value(net, "ip")?.ToString() ?? string.Empty,
                               Value(net, "mask")?.ToString() ?? string.Empty);
      port.NetworkTableOfInterface.Add(entity);
    }

    return port;
  }

  static object? Value(IDictionary<string, object?> dict, string key)
    => dict.TryGetValue(key, out var value) ? value : null;

  static int ToInt(object? value) => value switch
  {
    null => 0,
    int i => i,
    _ => Convert.ToInt32(value)
  };

  static long ToLong(object? value) => value switch
  {
    null => 0,
    long l => l,
    _ => Convert.ToInt64(value)
  };

  static List<int> ToIntList(object? value) => [.. AsList(value).Select(ToInt)];

  static List<IDictionary<string, object?>> AsDictList(object? value)
    => [.. AsList(value).OfType<IDictionary<string, object?>>()];

  /// <summary>
  /// Normalizes a Jint <c>ToObject()</c> array (or any other enumerable) into a plain list,
  /// regardless of the concrete collection type Jint happened to produce.
  /// </summary>
  static List<object?> AsList(object? value) => value switch
  {
    null => [],
    IEnumerable<object?> generic => [.. generic],
    System.Collections.IEnumerable nonGeneric and not string => [.. nonGeneric.Cast<object?>()],
    _ => []
  };
}
