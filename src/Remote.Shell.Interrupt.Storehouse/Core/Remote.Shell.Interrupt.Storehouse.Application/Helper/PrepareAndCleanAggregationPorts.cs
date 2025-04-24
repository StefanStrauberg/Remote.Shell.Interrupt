namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

// Ладно, эта штука отвечает за очистку и подготовку портов. Надеюсь, это не приведет к какой-нибудь чёртовой катастрофе!
internal static class PrepareAndCleanAggregationPorts
{
  internal static void Handle(IEnumerable<NetworkDevice> networkDevices)
  {
    foreach (var networkDevice in networkDevices)
    {
      // Ну вот, создаём хешсет для идентификаторов агрегированных портов... Потому что ничего другого не придумали.
      HashSet<Guid> aggregatedPortsIds = [];

      foreach (var port in networkDevice.PortsOfNetworkDevice
                                        .Where(x => x.ParentPortId is not null))
      {
        // Ага, находим родительский порт. Надеюсь, он не исчезнет в другой галактике!
        var parentPort = networkDevice.PortsOfNetworkDevice
                                      .First(x => x.Id == port.ParentPortId);
        parentPort.AggregatedPorts
                  .Add(port);
        aggregatedPortsIds.Add(port.Id);
      }

      if (aggregatedPortsIds.Count == 0)
        continue;

      // Очистим список портов. И да, сортировка—это как попытка навести порядок в хаосе.
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
                                                            .Where(port => !aggregatedPortsIds.Contains(port.Id))
                                                            .OrderBy(port => port.InterfaceName)];
    }
  }
}
