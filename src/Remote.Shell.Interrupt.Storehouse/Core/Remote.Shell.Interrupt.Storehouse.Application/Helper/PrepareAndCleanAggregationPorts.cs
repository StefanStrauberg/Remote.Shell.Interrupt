namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

internal static class PrepareAndCleanAggregationPorts
{
  internal static void Handle(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var networkDevice in networkDevices)
    {
      HashSet<Guid> aggregatedPortsIds = [];

      foreach (var port in networkDevice.PortsOfNetworkDevice.Where(x => x.ParentPortId is not null))
      {
        var parentPort = networkDevice.PortsOfNetworkDevice.First(x => x.Id == port.ParentPortId);
        parentPort.AggregatedPorts.Add(port);
        aggregatedPortsIds.Add(port.Id);
      }

      // Фильтруем PortsOfNetworkDevice, исключая повторяющиеся порты
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
          .Where(port => !aggregatedPortsIds.Contains(port.Id))
          .OrderBy(port => port.InterfaceName)];
    }
  }
}
