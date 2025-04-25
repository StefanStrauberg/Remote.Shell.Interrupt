namespace Remote.Shell.Interrupt.Storehouse.Application.Helper
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

        // Обрабатываем все порты, у которых задан родительский порт.
        foreach (var childPort in networkDevice.PortsOfNetworkDevice
                                               .Where(port => port.ParentPortId != null))
        {
          // Если родительский порт найден, добавляем childPort в агрегированные порты родителя.
          if (childPort.ParentPortId.HasValue &&
              portDictionary.TryGetValue(childPort.ParentPortId.Value, out var parentPort))
          {
            parentPort.AggregatedPorts.Add(childPort);
            aggregatedPortsIds.Add(childPort.Id);
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
