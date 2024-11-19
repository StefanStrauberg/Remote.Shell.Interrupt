
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class NetworkDeviceRepository(DapperContext context) : GenericRepository<NetworkDevice>(context), INetworkDeviceRepository
{
  public Task<IEnumerable<NetworkDevice>> FindManyWithChildrenAsync(Expression<Func<NetworkDevice, bool>> filterExpression, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<NetworkDevice>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public async Task<NetworkDevice> GetFirstWithChildrensByIdAsync(Guid id,
                                                                  CancellationToken cancellationToken)
  {
    string tableName = GetTableName();
    string columns = GetColumnsAsProperties();
    var connection = await _context.CreateConnectionAsync(cancellationToken);

    var query = $"SELECT " +
                 "nd.\"Id\", " +
                 "nd.\"Host\", " +
                 "nd.\"TypeOfNetworkDevice\", " +
                 "nd.\"NetworkDeviceName\", " +
                 "nd.\"GeneralInformation\", " +
                 "p.\"Id\", " +
                 "p.\"InterfaceNumber\", " +
                 "p.\"InterfaceName\", " +
                 "p.\"InterfaceType\", " +
                 "p.\"InterfaceStatus\", " +
                 "p.\"InterfaceSpeed\", " +
                 "p.\"NetworkDeviceId\", " +
                 "p.\"ParentPortId\", " +
                 "p.\"MACAddress\" " +
                 "FROM \"NetworkDevices\" as nd " +
                 "LEFT JOIN \"Ports\" AS p on p.\"NetworkDeviceId\" = nd.\"Id\" " +
                 "WHERE nd.\"Id\"=@Id";

    var networkDeviceDictionary = new Dictionary<Guid, NetworkDevice>();

    await connection.QueryAsync<NetworkDevice, Port, NetworkDevice>(
        query,
        (nd, p) =>
        {
          // Проверяем, существует ли NetworkDevice в словаре
          if (!networkDeviceDictionary.TryGetValue(nd.Id, out var networkDeviceEntry))
          {
            networkDeviceEntry = nd;
            networkDeviceEntry.PortsOfNetworkDevice = []; // Инициализация списка портов
            networkDeviceDictionary.Add(networkDeviceEntry.Id, networkDeviceEntry);
          }
          // Проверяем, существует ли Port, если да, добавляем его в PortsOfNetworkDevice
          if (p != null)
          {
            var portEntry = p;

            // Добавляем порт в устройство сети
            networkDeviceEntry.PortsOfNetworkDevice.Add(portEntry);
          }
          return networkDeviceEntry;
        },
        new { Id = id },
        splitOn: "Id");
    return networkDeviceDictionary.Values.First();
  }
}
