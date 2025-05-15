namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers
{
  // Класс для очистки и подготовки данных агрегированных портов.
  internal static class PrepareAndCleanAggregationPorts
  {
    internal static void Handle(IEnumerable<NetworkDevice> networkDevices)
    {
      foreach (var networkDevice in networkDevices)
      {
        // Создаем набор идентификаторов агрегированных портов.
        var aggregatedPortsIds = new HashSet<Guid>();

        // Создаем словарь для быстрого поиска порта по его идентификатору.
        var portDictionary = networkDevice.PortsOfNetworkDevice
                                          .ToDictionary(port => port.Id);

        var aggregatedPorts = networkDevice.PortsOfNetworkDevice
                                           .Where(port => port.ParentId != null);

        // Обрабатываем все порты, у которых задан родительский порт.
        foreach (var aggregatedPort in aggregatedPorts)
        {
          // Если родительский порт найден, добавляем childPort в агрегированные порты родителя.
          if (aggregatedPort.ParentId.HasValue &&
              portDictionary.TryGetValue(aggregatedPort.ParentId.Value, out var parentPort))
          {
            parentPort.AggregatedPorts.Add(aggregatedPort);
            aggregatedPortsIds.Add(aggregatedPort.Id);
          }
        }

        // Фильтруем и сортируем список портов: исключаем агрегированные дочерние порты и сортируем по имени интерфейса.
        networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice
                                                              .Where(port => !aggregatedPortsIds.Contains(port.Id))
                                                              .OrderBy(port => port.InterfaceName)];
      }
    }
  }
}
