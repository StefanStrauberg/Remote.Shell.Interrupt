using System.Collections;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.CreateNetworkDevice;

public record CreateNetworkDeviceCommand(string Host, string Community, string TypeOfNetworkDevice) : ICommand;

internal class CreateNetworkDeviceCommandHandler(ISNMPCommandExecutor snmpCommandExecutor,
                                                 INetDevUnitOfWork netDevUnitOfWork,
                                                 IConfiguration configuration)
  : ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  const string AeInterfacePrefix = "ae",
               XeInterfacePrefix = "xe",
               IfStackTableOid = "1.3.6.1.2.1.31.1.2.1.3",
               HuaweiIfStackTableOid = "1.2.840.10006.300.43.1.1.2.1.1",
               ExtremeIfStackTableOid = "1.3.6.1.4.1.1916.1.4.3.1.4",
               SystemDescriptionOid = "1.3.6.1.2.1.1.1.0",
               SystemNameOid = "1.3.6.1.2.1.1.5.0",
               InterfaceIndexOid = "1.3.6.1.2.1.2.2.1.1",
               InterfaceNameOid = "1.3.6.1.2.1.2.2.1.2",
               InterfaceTypeOid = "1.3.6.1.2.1.2.2.1.3",
               InterfaceSpeedOid = "1.3.6.1.2.1.2.2.1.5",
               InterfaceMacAddressOid = "1.3.6.1.2.1.2.2.1.6",
               InterfaceStatusOid = "1.3.6.1.2.1.2.2.1.8",
               InterfaceDescriptionOid = "1.3.6.1.2.1.31.1.1.1.18",
               ArpInterfaceIndexOid = "1.3.6.1.2.1.4.22.1.1",
               ArpMacAddressOid = "1.3.6.1.2.1.4.22.1.2",
               ArpIpAddressOid = "1.3.6.1.2.1.4.22.1.3",
               MacToVirtualPortOid = "1.3.6.1.2.1.17.4.3.1.2",
               IpAddressInterfaceIndexOid = "1.3.6.1.2.1.4.20.1.2",
               IpAddressOid = "1.3.6.1.2.1.4.20.1.1",
               SubnetMaskOid = "1.3.6.1.2.1.4.20.1.3",
               Dot1dBasePortOid = "1.3.6.1.2.1.17.1.4.1.1",
               Dot1dBasePortIfIndexOid = "1.3.6.1.2.1.17.1.4.1.2",
               Dot1qVlanStaticNameOid = "1.3.6.1.2.1.17.7.1.4.3.1.1",
               JuniperDot1qVlanStaticEgressPortsOid = "1.3.6.1.2.1.17.7.1.4.3.1.2",
               HuaweiDot1qVlanStaticEgressPortsOid = "1.3.6.1.2.1.17.7.1.4.2.1.4.0";
  const char MacAddressSeparator = ':',
             DotSeparator = '.';
  const int HuaweiPortThreshold = 48,
            HuaweiPortOffsetAboveThreshold = 12,
            HuaweiPortOffsetBelowThreshold = 6;

  async Task<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var networkDevice = new NetworkDevice
    {
      Id = Guid.NewGuid(),
      TypeOfNetworkDevice = Enum.Parse<TypeOfNetworkDevice>(request.TypeOfNetworkDevice),
      Host = request.Host
    };

    int maxRepetitions = GetRepetitionConfigurationKey(networkDevice.TypeOfNetworkDevice);

    await FillNetworkDeviceGeneralInformation(networkDevice, request.Host, request.Community, cancellationToken);
    await FillNetworkDeviceName(networkDevice, request.Host, request.Community, cancellationToken);
    await FillPortsOfNetworkDevice(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
    await FillArpTableForPorts(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
    await FillMacTableForPorts(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
    await FillNetworkTableOfPort(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);

    if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Juniper)
    {
      await FillPortVlansForJuniper(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);

      var removingPorts = GetUseLessPorts(networkDevice.PortsOfNetworkDevice);
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice.Where(x => !removingPorts.Contains(x))];

      await LinkAgregationPortsForJuniper(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
      RemoveDotNotationPorts(networkDevice.PortsOfNetworkDevice);
    }
    else if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Huawei)
    {
      await FillPortVlansForHuawei(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
      await LinkAggregatedPortsForHuawei(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);

      var removingPorts = CleanHuawei(networkDevice.PortsOfNetworkDevice);
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice.Where(x => !removingPorts.Contains(x))];
    }
    else if (networkDevice.TypeOfNetworkDevice == TypeOfNetworkDevice.Extreme)
    {
      await FillPortVLANSForExtreme(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);

      var removingPorts = CleanExtreme(networkDevice.PortsOfNetworkDevice);
      networkDevice.PortsOfNetworkDevice = [.. networkDevice.PortsOfNetworkDevice.Where(x => !removingPorts.Contains(x))];

      await LinkAggregatedPortsForExtreme(networkDevice, request.Host, request.Community, maxRepetitions, cancellationToken);
    }

    netDevUnitOfWork.NetworkDevices.InsertOne(networkDevice);
    netDevUnitOfWork.Complete();

    return Unit.Value;
  }

  int GetRepetitionConfigurationKey(TypeOfNetworkDevice deviceType)
    => deviceType switch
    {
      TypeOfNetworkDevice.Juniper => configuration.GetValue<int>("Repetitions:Juniper"),
      TypeOfNetworkDevice.Huawei => configuration.GetValue<int>("Repetitions:Huawei"),
      TypeOfNetworkDevice.Extreme => configuration.GetValue<int>("Repetitions:Extreme"),
      _ => configuration.GetValue<int>("Repetitions:Default")
    };

  async Task FillNetworkDeviceGeneralInformation(NetworkDevice networkDevice, string host, string community, CancellationToken cancellationToken)
  {
    var systemDescription = await RetrieveSystemDescription(host, community, cancellationToken);
    networkDevice.GeneralInformation = systemDescription;
  }

  async Task<string> RetrieveSystemDescription(string host, string community, CancellationToken cancellationToken)
  {
    var response = await snmpCommandExecutor.GetCommand(host, community, SystemDescriptionOid, cancellationToken);
    return response.Data;
  }

  async Task FillNetworkDeviceName(NetworkDevice networkDevice, string host, string community, CancellationToken cancellationToken)
  {
    var deviceName = await RetrieveSystemName(host, community, cancellationToken);
    networkDevice.NetworkDeviceName = deviceName;
  }

  async Task<string> RetrieveSystemName(string host, string community, CancellationToken cancellationToken)
  {
    var response = await snmpCommandExecutor.GetCommand(host, community, SystemNameOid, cancellationToken);
    return response.Data;
  }

  async Task FillPortsOfNetworkDevice(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var interfaceData = await RetrieveAllInterfaceData(host, community, maxRepetitions, cancellationToken);

    if (interfaceData.Count == 0) return;

    var ports = CreatePortsFromInterfaceData(interfaceData, networkDevice.Id);
    networkDevice.PortsOfNetworkDevice = ports;
  }

  async Task<List<InterfaceData>> RetrieveAllInterfaceData(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var interfaceNumbers = await RetrieveInterfaceNumbers(host, community, maxRepetitions, cancellationToken);
    var interfaceNames = await RetrieveInterfaceNames(host, community, maxRepetitions, cancellationToken);
    var interfaceTypes = await RetrieveInterfaceTypes(host, community, maxRepetitions, cancellationToken);
    var interfaceSpeeds = await RetrieveInterfaceSpeeds(host, community, maxRepetitions, cancellationToken);
    var interfaceMacAddresses = await RetrieveInterfaceMacAddresses(host, community, maxRepetitions, cancellationToken);
    var interfaceStatuses = await RetrieveInterfaceStatuses(host, community, maxRepetitions, cancellationToken);
    var interfaceDescriptions = await RetrieveInterfaceDescriptions(host, community, maxRepetitions, cancellationToken);

    return CombineInterfaceData(interfaceNumbers, interfaceNames, interfaceTypes, interfaceSpeeds, interfaceMacAddresses, interfaceStatuses, interfaceDescriptions);
  }

  async Task<List<SNMPResponse>> RetrieveInterfaceNumbers(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceIndexOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceNames(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceNameOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceTypes(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceTypeOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceSpeeds(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceSpeedOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceMacAddresses(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceMacAddressOid, cancellationToken, toHex: true, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceStatuses(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceStatusOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveInterfaceDescriptions(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, InterfaceDescriptionOid, cancellationToken, repetitions: maxRepetitions);

  static List<InterfaceData> CombineInterfaceData(List<SNMPResponse> numbers, List<SNMPResponse> names, List<SNMPResponse> types, List<SNMPResponse> speeds, List<SNMPResponse> macAddresses, List<SNMPResponse> statuses, List<SNMPResponse> descriptions)
    => [.. numbers.Zip(names, (number, name) => new { Number = int.Parse(number.Data), Name = name.Data })
                  .Zip(types, (first, type) => new { first.Number, first.Name, Type = Enum.Parse<PortType>(type.Data) })
                  .Zip(speeds, (second, speed) => new { second.Number, second.Name, second.Type, Speed = long.Parse(speed.Data) })
                  .Zip(statuses, (third, status) => new { third.Number, third.Name, third.Type, third.Speed, Status = Enum.Parse<PortStatus>(status.Data) })
                  .Zip(macAddresses, (fourth, mac) => new { fourth.Number, fourth.Name, fourth.Type, fourth.Speed, fourth.Status, Mac = mac.Data.Replace(' ', MacAddressSeparator) })
                  .Zip(descriptions, (fifth, description) => new InterfaceData{ Number = fifth.Number, Name = fifth.Name, Type = fifth.Type, Speed = fifth.Speed, Status = fifth.Status, MacAddress = fifth.Mac, Description = description.Data })];

  static List<Port> CreatePortsFromInterfaceData(List<InterfaceData> interfaceData, Guid networkDeviceId)
  {
    var ports = new List<Port>(interfaceData.Count);

    foreach (var data in interfaceData)
      ports.Add(new Port { Id = Guid.NewGuid(), NetworkDeviceId = networkDeviceId, InterfaceNumber = data.Number, InterfaceName = data.Name, InterfaceSpeed = data.Speed, InterfaceStatus = data.Status, MACAddress = data.MacAddress, Description = data.Description, InterfaceType = data.Type });

    return ports;
  }

  async Task FillMacTableForPorts(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var (macToVirtualPorts, virtualPorts, virtualPortToInterfaces) = await RetrieveMacTableData(host, community, maxRepetitions, cancellationToken);

    if (!HasValidMacTableData(macToVirtualPorts, virtualPorts, virtualPortToInterfaces)) return;

    ValidateVirtualPortDataConsistency(virtualPorts, virtualPortToInterfaces);

    var macTable = BuildMacAddressTable(macToVirtualPorts);
    var interfaceMapping = BuildInterfaceMapping(virtualPorts, virtualPortToInterfaces);

    AssignMacAddressesToPorts(networkDevice.PortsOfNetworkDevice, macTable, interfaceMapping);
  }

  async Task<(List<SNMPResponse> MacToVirtualPorts, List<SNMPResponse> VirtualPorts, List<SNMPResponse> VirtualPortToInterfaces)> RetrieveMacTableData(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var macToVirtualPorts = await RetrieveMacToVirtualPorts(host, community, maxRepetitions, cancellationToken);
    var virtualPorts = await RetrieveVirtualPortNumbers(host, community, maxRepetitions, cancellationToken);
    var virtualPortToInterfaces = await RetrieveVirtualPortToInterfaces(host, community, maxRepetitions, cancellationToken);

    return (macToVirtualPorts, virtualPorts, virtualPortToInterfaces);
  }

  async Task<List<SNMPResponse>> RetrieveMacToVirtualPorts(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, MacToVirtualPortOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveVirtualPortNumbers(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, Dot1dBasePortOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveVirtualPortToInterfaces(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, Dot1dBasePortIfIndexOid, cancellationToken, repetitions: maxRepetitions);

  static bool HasValidMacTableData(List<SNMPResponse> macToVirtualPorts, List<SNMPResponse> virtualPorts, List<SNMPResponse> virtualPortToInterfaces)
    => macToVirtualPorts.Count > 0 && virtualPorts.Count > 0 && virtualPortToInterfaces.Count > 0;

  static void ValidateVirtualPortDataConsistency(List<SNMPResponse> virtualPorts, List<SNMPResponse> virtualPortToInterfaces)
  {
    if (virtualPorts.Count != virtualPortToInterfaces.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for virtual port data: virtual ports({virtualPorts.Count}), virtual port to interface({virtualPortToInterfaces.Count})");
  }

  static Dictionary<int, List<string>> BuildMacAddressTable(List<SNMPResponse> macToVirtualPorts)
    => macToVirtualPorts.Select(response => new { VirtualPortNumber = int.Parse(response.Data), MacAddress = FormatMACAddress.HandleMACTable(response.OID) })
                        .GroupBy(entry => entry.VirtualPortNumber)
                        .ToDictionary(group => group.Key, group => group.Select(x => x.MacAddress).ToList());

  static Dictionary<int, int> BuildInterfaceMapping(List<SNMPResponse> virtualPorts, List<SNMPResponse> virtualPortToInterfaces)
    => virtualPorts.Zip(virtualPortToInterfaces, (virtualPort, interfaceNum) => new { VirtualPortNumber = int.Parse(virtualPort.Data), InterfaceNumber = int.Parse(interfaceNum.Data) })
                   .ToDictionary(entry => entry.VirtualPortNumber, entry => entry.InterfaceNumber);

  static void AssignMacAddressesToPorts(IEnumerable<Port> ports, Dictionary<int, List<string>> macTable, Dictionary<int, int> interfaceMapping)
  {
    var portsDictionary = ports.ToDictionary(port => port.InterfaceNumber);

    foreach (var (virtualPortNumber, macAddresses) in macTable)
    {
      if (interfaceMapping.TryGetValue(virtualPortNumber, out var interfaceNumber) && portsDictionary.TryGetValue(interfaceNumber, out var port))
        AddMacAddressesToPort(port, macAddresses);
    }
  }

  static void AddMacAddressesToPort(Port port, List<string> macAddresses)
  {
    foreach (var macAddress in macAddresses)
      port.MACTable.Add(new MACEntity { Id = Guid.NewGuid(), MACAddress = macAddress, PortId = port.Id });
  }

  static List<Port> GetUseLessPorts(List<Port> portsOfNetworkDevice)
    => [.. portsOfNetworkDevice.Where(port => !(port.InterfaceName.StartsWith("xe") || port.InterfaceName.StartsWith("irb") || port.InterfaceName.StartsWith("ae")))];

  static void RemoveDotNotationPorts(List<Port> ports)
  {
    var portsToRemove = IdentifyDotNotationPorts(ports);
    RemovePortsFromCollection(ports, portsToRemove);
  }

  static List<Port> IdentifyDotNotationPorts(List<Port> ports)
  {
    var basePortNames = GetBasePortNamesWithoutDots(ports);
    return FindAllDotNotationPorts(ports, basePortNames);
  }

  static HashSet<string> GetBasePortNamesWithoutDots(List<Port> ports)
    => [.. ports.Where(port => (port.InterfaceName.StartsWith(AeInterfacePrefix) || port.InterfaceName.StartsWith(XeInterfacePrefix)) && !port.InterfaceName.Contains(DotSeparator))
                .Select(port => port.InterfaceName)];

  static List<Port> FindAllDotNotationPorts(List<Port> ports, HashSet<string> basePortNames)
  {
    var portsToRemove = new List<Port>();

    foreach (var baseName in basePortNames)
    {
      var dotNotationPattern = baseName + DotSeparator;
      var dotPorts = ports.Where(port => port.InterfaceName.StartsWith(dotNotationPattern));
      portsToRemove.AddRange(dotPorts);
    }

    return portsToRemove;
  }

  static void RemovePortsFromCollection(List<Port> ports, List<Port> portsToRemove)
  {
    foreach (var portToRemove in portsToRemove)
      ports.Remove(portToRemove);
  }

  static IEnumerable<Port> CleanHuawei(List<Port> portsOfNetworkDevice)
    => portsOfNetworkDevice.Where(port => port.InterfaceName.StartsWith("InLoop") || port.InterfaceName.StartsWith("MEth") || port.InterfaceName.StartsWith("NULL0"));

  static IEnumerable<Port> CleanExtreme(List<Port> portsOfNetworkDevice)
    => portsOfNetworkDevice.Where(port => port.InterfaceName.StartsWith("Management") || port.InterfaceName.StartsWith("Virtual") || port.InterfaceName.StartsWith("VLAN"));

  async Task FillArpTableForPorts(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var (interfaceNumbers, macAddresses, ipAddresses) = await RetrieveArpData(host, community, maxRepetitions, cancellationToken);

    ValidateArpDataConsistency(interfaceNumbers, macAddresses, ipAddresses);

    var arpEntries = CombineArpEntries(interfaceNumbers, macAddresses, ipAddresses);
    var arpDictionary = GroupArpEntriesByInterface(arpEntries);

    AssignArpTablesToPorts(networkDevice.PortsOfNetworkDevice, arpDictionary);
  }

  async Task<(List<SNMPResponse> InterfaceNumbers, List<SNMPResponse> MacAddresses, List<SNMPResponse> IpAddresses)> RetrieveArpData(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var interfaceNumbers = await RetrieveArpInterfaceNumbers(host, community, maxRepetitions, cancellationToken);
    var macAddresses = await RetrieveArpMacAddresses(host, community, maxRepetitions, cancellationToken);
    var ipAddresses = await RetrieveArpIpAddresses(host, community, maxRepetitions, cancellationToken);

    return (interfaceNumbers, macAddresses, ipAddresses);
  }

  async Task<List<SNMPResponse>> RetrieveArpInterfaceNumbers(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, ArpInterfaceIndexOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveArpMacAddresses(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, ArpMacAddressOid, cancellationToken, toHex: true, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveArpIpAddresses(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, ArpIpAddressOid, cancellationToken, repetitions: maxRepetitions);

  static void ValidateArpDataConsistency(List<SNMPResponse> interfaceNumbers, List<SNMPResponse> macAddresses, List<SNMPResponse> ipAddresses)
  {
    if (interfaceNumbers.Count == 0 || macAddresses.Count == 0 || ipAddresses.Count == 0)
      throw new InvalidOperationException("One or more SNMP requests returned empty results for ARP data.");

    if (interfaceNumbers.Count != macAddresses.Count || macAddresses.Count != ipAddresses.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for ARP data: " + $"interfaces({interfaceNumbers.Count}), macs({macAddresses.Count}), ips({ipAddresses.Count})");
  }

  static List<ArpEntry> CombineArpEntries(List<SNMPResponse> interfaceNumbers, List<SNMPResponse> macAddresses, List<SNMPResponse> ipAddresses)
    => [.. interfaceNumbers.Zip(macAddresses, (iface, mac) => new { InterfaceNumber = int.Parse(iface.Data), MacAddress = FormatMACAddress.Handle(mac.Data) })
                           .Zip(ipAddresses, (firstPair, ip) => new ArpEntry { InterfaceNumber = firstPair.InterfaceNumber, MacAddress = firstPair.MacAddress, IpAddress = ip.Data })];

  static Dictionary<int, List<ArpEntry>> GroupArpEntriesByInterface(List<ArpEntry> arpEntries)
    => arpEntries.GroupBy(entry => entry.InterfaceNumber)
                 .ToDictionary(group => group.Key, group => group.ToList());

  static void AssignArpTablesToPorts(IEnumerable<Port> ports, Dictionary<int, List<ArpEntry>> arpDictionary)
  {
    var portsDictionary = ports.ToDictionary(port => port.InterfaceNumber);

    foreach (var (interfaceNumber, arpEntries) in arpDictionary)
    {
      if (portsDictionary.TryGetValue(interfaceNumber, out var port))
        port.ARPTableOfInterface = CreateArpTable(arpEntries, port.Id);
    }
  }

  static List<ARPEntity> CreateArpTable(List<ArpEntry> arpEntries, Guid portId)
    => [.. arpEntries.Select(entry => new ARPEntity { Id = Guid.NewGuid(), MAC = entry.MacAddress, IPAddress = entry.IpAddress, PortId = portId })];

  async Task FillNetworkTableOfPort(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var (interfaceNumbers, ipAddresses, subnetMasks) = await RetrieveNetworkTableData(host, community, maxRepetitions, cancellationToken);

    ValidateNetworkTableDataConsistency(interfaceNumbers, ipAddresses, subnetMasks);

    var networkEntries = CombineNetworkEntries(interfaceNumbers, ipAddresses, subnetMasks);
    var networkDictionary = GroupNetworkEntriesByInterface(networkEntries);

    AssignNetworkTablesToPorts(networkDevice.PortsOfNetworkDevice, networkDictionary);
  }

  async Task<(List<SNMPResponse> InterfaceNumbers, List<SNMPResponse> IpAddresses, List<SNMPResponse> SubnetMasks)> RetrieveNetworkTableData(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var interfaceNumbers = await RetrieveInterfaceNumbersOfNetworkTable(host, community, maxRepetitions, cancellationToken);
    var ipAddresses = await RetrieveIpAddresses(host, community, maxRepetitions, cancellationToken);
    var subnetMasks = await RetrieveSubnetMasks(host, community, maxRepetitions, cancellationToken);

    return (interfaceNumbers, ipAddresses, subnetMasks);
  }

  async Task<List<SNMPResponse>> RetrieveInterfaceNumbersOfNetworkTable(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, IpAddressInterfaceIndexOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveIpAddresses(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, IpAddressOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveSubnetMasks(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, SubnetMaskOid, cancellationToken, repetitions: maxRepetitions);

  static void ValidateNetworkTableDataConsistency(List<SNMPResponse> interfaceNumbers, List<SNMPResponse> ipAddresses, List<SNMPResponse> subnetMasks)
  {
    if (interfaceNumbers.Count == 0 || ipAddresses.Count == 0 || subnetMasks.Count == 0)
      throw new InvalidOperationException("One or more SNMP requests returned empty results for network table data.");

    if (interfaceNumbers.Count != ipAddresses.Count || ipAddresses.Count != subnetMasks.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch for network table data: interfaces({interfaceNumbers.Count}), IP addresses({ipAddresses.Count}), subnet masks({subnetMasks.Count})");
  }

  static List<NetworkEntry> CombineNetworkEntries(List<SNMPResponse> interfaceNumbers, List<SNMPResponse> ipAddresses, List<SNMPResponse> subnetMasks)
    => [.. interfaceNumbers.Zip(ipAddresses, (iface, ip) => new { InterfaceNumber = int.Parse(iface.Data), IpAddress = ip.Data })
                           .Zip(subnetMasks, (firstPair, mask) => new NetworkEntry { InterfaceNumber = firstPair.InterfaceNumber, IpAddress = firstPair.IpAddress, SubnetMask = mask.Data })];

  static Dictionary<int, List<NetworkEntry>> GroupNetworkEntriesByInterface(List<NetworkEntry> networkEntries)
    => networkEntries.GroupBy(entry => entry.InterfaceNumber).ToDictionary(group => group.Key, group => group.ToList());

  static void AssignNetworkTablesToPorts(IEnumerable<Port> ports, Dictionary<int, List<NetworkEntry>> networkDictionary)
  {
    var portsDictionary = ports.ToDictionary(port => port.InterfaceNumber);

    foreach (var (interfaceNumber, networkEntries) in networkDictionary)
    {
      if (portsDictionary.TryGetValue(interfaceNumber, out var port))
        port.NetworkTableOfInterface = CreateNetworkTable(networkEntries, port.Id);
    }
  }

  static List<TerminatedNetworkEntity> CreateNetworkTable(List<NetworkEntry> networkEntries, Guid portId)
  {
    var networkTable = new List<TerminatedNetworkEntity>();

    foreach (var entry in networkEntries)
    {
      var terminatedNetwork = new TerminatedNetworkEntity { Id = Guid.NewGuid(), PortId = portId };
      terminatedNetwork.SetAddressAndMask(entry.IpAddress, entry.SubnetMask);
      networkTable.Add(terminatedNetwork);
    }

    return networkTable;
  }

  async Task FillPortVlansForJuniper(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var (basePorts, portIfIndexes, vlanNames, vlanEgressPorts) = await RetrieveVlanData(networkDevice, host, community, maxRepetitions, cancellationToken);

    ValidateVlanDataConsistency(basePorts, portIfIndexes, vlanNames, vlanEgressPorts);

    var physicalInterfaceTable = BuildPhysicalInterfaceTable(basePorts, portIfIndexes);
    var vlanTableEntries = BuildVlanTableEntries(vlanNames, vlanEgressPorts, TypeOfNetworkDevice.Juniper);

    AssignVlansToPorts(networkDevice.PortsOfNetworkDevice, physicalInterfaceTable, vlanTableEntries);
  }

  async Task<(List<SNMPResponse> BasePorts, List<SNMPResponse> PortIfIndexes, List<SNMPResponse> VlanNames, List<SNMPResponse> VlanEgressPorts)> RetrieveVlanData(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var basePorts = await RetrieveBasePorts(host, community, maxRepetitions, cancellationToken);
    var portIfIndexes = await RetrievePortIfIndexes(host, community, maxRepetitions, cancellationToken);
    var vlanNames = await RetrieveVlanNames(host, community, maxRepetitions, cancellationToken);
    var vlanEgressPorts = new List<SNMPResponse>();

    switch (networkDevice.TypeOfNetworkDevice)
    {
      case TypeOfNetworkDevice.Juniper:
        vlanEgressPorts = await RetrieveVlanEgressPortsJuniper(host, community, maxRepetitions, cancellationToken);
        break;
      case TypeOfNetworkDevice.Huawei:
        vlanEgressPorts = await RetrieveVlanEgressPortsHuawei(host, community, maxRepetitions, cancellationToken);
        break;
      case TypeOfNetworkDevice.Extreme:
        break;
    }

    return (basePorts, portIfIndexes, vlanNames, vlanEgressPorts);
  }

  async Task<List<SNMPResponse>> RetrieveBasePorts(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, Dot1dBasePortOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrievePortIfIndexes(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, Dot1dBasePortIfIndexOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveVlanNames(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, Dot1qVlanStaticNameOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveVlanEgressPortsJuniper(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, JuniperDot1qVlanStaticEgressPortsOid, cancellationToken, repetitions: maxRepetitions);

  async Task<List<SNMPResponse>> RetrieveVlanEgressPortsHuawei(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, HuaweiDot1qVlanStaticEgressPortsOid, cancellationToken, repetitions: maxRepetitions);

  static void ValidateVlanDataConsistency(List<SNMPResponse> basePorts, List<SNMPResponse> portIfIndexes, List<SNMPResponse> vlanNames, List<SNMPResponse> vlanEgressPorts)
  {
    if (basePorts.Count == 0 || portIfIndexes.Count == 0 || vlanNames.Count == 0 || vlanEgressPorts.Count == 0)
      throw new InvalidOperationException("One or more SNMP requests returned empty results for VLAN data.");

    if (basePorts.Count != portIfIndexes.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch: dot1dBasePortIfIndex({basePorts.Count}) != dot1dBasePortIfIndex({portIfIndexes.Count})");

    if (vlanNames.Count != vlanEgressPorts.Count)
      throw new InvalidOperationException($"SNMP responses count mismatch: dot1qVlanStaticName({vlanNames.Count}) != dot1qVlanStaticEgressPorts({vlanEgressPorts.Count})");
  }

  static Dictionary<int, int> BuildPhysicalInterfaceTable(List<SNMPResponse> basePorts, List<SNMPResponse> portIfIndexes)
    => basePorts.Zip(portIfIndexes, (basePort, ifIndex) => new
    {
      BasePort = int.Parse(basePort.Data),
      PortIfIndex = int.Parse(ifIndex.Data)
    }).ToDictionary(x => x.BasePort, x => x.PortIfIndex);

  static List<VlanTableEntry> BuildVlanTableEntries(List<SNMPResponse> vlanNames, List<SNMPResponse> vlanEgressPorts, TypeOfNetworkDevice typeOfNetworkDevice)
    => [.. vlanNames.Zip(vlanEgressPorts, (vlanName, egressPorts) => new VlanTableEntry
                                          {
                                            VlanTag = OIDGetNumbers.HandleLast(vlanName.OID),
                                            VlanName = RemoveTrailingPlusDigit.Handle(vlanName.Data),
                                            EgressPorts = typeOfNetworkDevice switch
                                            {
                                              TypeOfNetworkDevice.Juniper => FormatEgressPorts.HandleJuniperData(egressPorts.Data),
                                              TypeOfNetworkDevice.Huawei => FormatEgressPorts.HandleHuaweiHexStringOld(egressPorts.Data),
                                              _ => throw new InvalidOperationException("")
                                            }
                                          })];

  static void AssignVlansToPorts(IEnumerable<Port> ports, Dictionary<int, int> physicalInterfaceTable, List<VlanTableEntry> vlanTableEntries)
  {
    var portsDictionary = ports.ToDictionary(port => port.InterfaceNumber);
    var vlansToAdd = new HashSet<VLAN>();

    foreach (var vlanEntry in vlanTableEntries)
    {
      var vlan = CreateVlan(vlanEntry);
      vlansToAdd.Add(vlan);

      AssignVlanToPorts(vlan, vlanEntry.EgressPorts, physicalInterfaceTable, portsDictionary);
    }
  }

  static VLAN CreateVlan(VlanTableEntry vlanEntry)
    => new() { Id = Guid.NewGuid(), VLANTag = vlanEntry.VlanTag, VLANName = vlanEntry.VlanName };

  static void AssignVlanToPorts(VLAN vlan, IEnumerable<int> egressPorts, Dictionary<int, int> physicalInterfaceTable, Dictionary<int, Port> portsDictionary)
  {
    foreach (var egressPort in egressPorts)
    {
      if (physicalInterfaceTable.TryGetValue(egressPort, out var portIfIndex) && portsDictionary.TryGetValue(portIfIndex, out var port))
      {
        port.VLANs ??= [];
        port.VLANs.Add(vlan);
      }
    }
  }

  async Task LinkAgregationPortsForJuniper(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var ifStackTable = await RetrieveIfStackTable(host, community, maxRepetitions, cancellationToken);
    if (ifStackTable.Count == 0) return;

    var aePorts = FilterPortsByInterfacePrefix(networkDevice.PortsOfNetworkDevice, AeInterfacePrefix);
    var xePorts = FilterPortsByInterfacePrefix(networkDevice.PortsOfNetworkDevice, XeInterfacePrefix);

    var aePortGroups = GroupPorts(aePorts);
    var xePortGroups = GroupPorts(xePorts);

    var aggregationLinks = ExtractAggregationLinksFromIfStackTable(ifStackTable, aePortGroups);

    foreach (var link in aggregationLinks)
    {
      EstablishAggregationLink(link, aePortGroups, xePortGroups);
    }
  }

  async Task<List<SNMPResponse>> RetrieveIfStackTable(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, IfStackTableOid, cancellationToken, repetitions: maxRepetitions);

  static List<Port> FilterPortsByInterfacePrefix(IEnumerable<Port> ports, string prefix)
    => [.. ports.Where(port => port.InterfaceName.StartsWith(prefix))];

  static HashSet<(int AeNumber, int PortNumber)> ExtractAggregationLinksFromIfStackTable(List<SNMPResponse> ifStackTable, Dictionary<HashSet<int>, List<Port>> aePortGroups)
  {
    var aeKeys = aePortGroups.SelectMany(group => group.Key).ToHashSet();

    return [.. ifStackTable.Select(response => (AeNumber: OIDGetNumbers.HandleLastButOne(response.OID), PortNumber: OIDGetNumbers.HandleLast(response.OID)))
                           .Where(link => link.AeNumber != 0 && link.PortNumber != 0)
                           .Where(link => aeKeys.Contains(link.AeNumber))];
  }

  static void EstablishAggregationLink((int AeNumber, int PortNumber) link, Dictionary<HashSet<int>, List<Port>> aePortGroups, Dictionary<HashSet<int>, List<Port>> xePortGroups)
  {
    var aggregationPort = FindPortByNumber(aePortGroups, link.AeNumber);
    var memberPort = FindPortByNumber(xePortGroups, link.PortNumber);

    if (aggregationPort == null || memberPort == null) return;

    if (!aggregationPort.AggregatedPorts.Contains(memberPort))
    {
      aggregationPort.AggregatedPorts.Add(memberPort);
      memberPort.ParentId = aggregationPort.Id;
    }
  }

  static Port? FindPortByNumber(Dictionary<HashSet<int>, List<Port>> portGroups, int portNumber)
    => portGroups.FirstOrDefault(group => group.Key.Contains(portNumber)).Value.FirstOrDefault();

  async Task LinkAggregatedPortsForHuawei(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var ifStackTable = await RetrieveIfStackTable(host, community, maxRepetitions, cancellationToken);

    if (ifStackTable.Count == 0)
      await ProcessHuaweiSpecificAggregation(networkDevice, host, community, maxRepetitions, cancellationToken);
  }

  async Task ProcessHuaweiSpecificAggregation(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var huaweiIfStackTable = await RetrieveHuaweiIfStackTable(host, community, maxRepetitions, cancellationToken);

    if (huaweiIfStackTable.Count == 0) return;

    var aggregationLinks = ExtractHuaweiAggregationLinks(huaweiIfStackTable);
    EstablishAggregationLinks(networkDevice.PortsOfNetworkDevice, aggregationLinks);
  }

  static List<(int AeNumber, int[] PortNumbers)> ExtractHuaweiAggregationLinks(List<SNMPResponse> huaweiIfStackTable)
    => [.. huaweiIfStackTable.Select(response => (AeNumber: OIDGetNumbers.HandleLast(response.OID), PortNumbers: FormatEgressPorts.HandleHuaweiHexStringOld(response.Data)))
                             .Where(link => link.PortNumbers.Length > 0)];

  async Task<List<SNMPResponse>> RetrieveHuaweiIfStackTable(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, HuaweiIfStackTableOid, cancellationToken, toHex: true, repetitions: maxRepetitions);

  static void EstablishAggregationLinks(IEnumerable<Port> ports, List<(int AeNumber, int[] PortNumbers)> aggregationLinks)
  {
    var portsDictionary = ports.ToDictionary(p => p.InterfaceNumber);

    foreach (var (aeNumber, portNumbers) in aggregationLinks)
    {
      if (portsDictionary.TryGetValue(aeNumber, out var aggregationPort))
      {
        foreach (var portNumber in portNumbers)
        {
          var actualPortNumber = CalculateActualPortNumber(portNumber);

          if (portsDictionary.TryGetValue(actualPortNumber, out var memberPort))
            LinkPortToAggregation(memberPort, aggregationPort);
        }
      }
    }
  }

  static int CalculateActualPortNumber(int originalPortNumber)
    => originalPortNumber > HuaweiPortThreshold ? originalPortNumber + HuaweiPortOffsetAboveThreshold : originalPortNumber + HuaweiPortOffsetBelowThreshold;

  static void LinkPortToAggregation(Port memberPort, Port aggregationPort)
  {
    if (!aggregationPort.AggregatedPorts.Contains(memberPort))
    {
      aggregationPort.AggregatedPorts.Add(memberPort);
      memberPort.ParentId = aggregationPort.Id;
    }
  }

  async Task LinkAggregatedPortsForExtreme(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var ifStackTable = await RetrieveExtremeIfStackTable(host, community, maxRepetitions, cancellationToken);

    if (ifStackTable.Count == 0) return;

    var portsDictionary = networkDevice.PortsOfNetworkDevice.ToDictionary(port => port.InterfaceNumber);
    var aggregationLinks = ExtractExtremeAggregationLinks(ifStackTable);

    EstablishAggregationLinks(portsDictionary, aggregationLinks);
  }

  async Task<List<SNMPResponse>> RetrieveExtremeIfStackTable(string host, string community, int maxRepetitions, CancellationToken cancellationToken)
    => await snmpCommandExecutor.WalkCommand(host, community, ExtremeIfStackTableOid, cancellationToken, repetitions: maxRepetitions);

  static HashSet<(int AggregationPortNumber, int MemberPortNumber)> ExtractExtremeAggregationLinks(List<SNMPResponse> ifStackTable)
    => [.. ifStackTable.Select(response => (AggregationPortNumber: OIDGetNumbers.HandleLastButOne(response.OID), MemberPortNumber: OIDGetNumbers.HandleLast(response.OID)))
                       .Where(link => link.AggregationPortNumber != 0 && link.MemberPortNumber != 0 && link.AggregationPortNumber != link.MemberPortNumber)];

  static void EstablishAggregationLinks(Dictionary<int, Port> portsDictionary, HashSet<(int AggregationPortNumber, int MemberPortNumber)> aggregationLinks)
  {
    foreach (var (aggregationPortNumber, memberPortNumber) in aggregationLinks)
    {
      if (portsDictionary.TryGetValue(aggregationPortNumber, out var aggregationPort) &&
          portsDictionary.TryGetValue(memberPortNumber, out var memberPort))
      {
        if (!IsPortAlreadyAggregated(aggregationPort, memberPort))
          LinkPortToAggregation(memberPort, aggregationPort);
      }
    }
  }

  static bool IsPortAlreadyAggregated(Port aggregationPort, Port memberPort)
    => aggregationPort.AggregatedPorts.Any(x => x.InterfaceNumber == memberPort.InterfaceNumber);

  static Dictionary<HashSet<int>, List<Port>> GroupPorts(List<Port> ports)
  {
    // Создаем словарь для группировки
    var baseGroups = new Dictionary<HashSet<int>, List<Port>>();
    var lookingFor = new StringBuilder();

    // Находим все порты без точки и создаем группы
    var portsWithoutDots = ports.Where(port => !port.InterfaceName.Contains('.')).ToList();

    foreach (var port in portsWithoutDots)
    {
      var group = new List<Port>();
      var keys = new HashSet<int>(); // Начальный ключ

      lookingFor.Append(port.InterfaceName);
      lookingFor.Append('.');
      keys.Add(port.InterfaceNumber);
      group.Add(port);

      // Находим порты с точкой и добавляем их в группу
      var lookingGroupWithDot = ports.Where(p => p.InterfaceName.StartsWith(lookingFor.ToString())).ToList();
      group.AddRange(lookingGroupWithDot);

      foreach (var key in lookingGroupWithDot.Select(p => p.InterfaceNumber))
        keys.Add(key); // Добавляем ключи 

      // Добавляем группу в словарь
      baseGroups.Add(keys, group);
      lookingFor.Clear();
    }

    return baseGroups;
  }

  async Task FillPortVlansForHuawei(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken cancellationToken)
  {
    var (basePorts, portIfIndexes, vlanNames, vlanEgressPorts) = await RetrieveVlanData(networkDevice, host, community, maxRepetitions, cancellationToken);

    ValidateVlanDataConsistency(basePorts, portIfIndexes, vlanNames, vlanEgressPorts);

    var physicalInterfaceTable = BuildPhysicalInterfaceTable(basePorts, portIfIndexes);
    var vlanTableEntries = BuildVlanTableEntries(vlanNames, vlanEgressPorts, TypeOfNetworkDevice.Huawei);

    AssignVlansToPorts(networkDevice.PortsOfNetworkDevice, physicalInterfaceTable, vlanTableEntries);
  }

  async Task FillPortVLANSForExtreme(NetworkDevice networkDevice, string host, string community, int maxRepetitions, CancellationToken token)
  {
    List<SNMPResponse> portsToVlans = [];
    List<SNMPResponse> vlansNames = [];
    List<SNMPResponse> vlansNumbers = [];
    List<SNMPResponse> vlansTags = [];
    // Выполняем SNMP-запрос для получения VlanNumber
    vlansNumbers = await snmpCommandExecutor.WalkCommand(host, community, "1.3.6.1.4.1.1916.1.2.1.2.1.1", token, repetitions: maxRepetitions);
    // Выполняем SNMP-запрос для получения VlanName
    vlansNames = await snmpCommandExecutor.WalkCommand(host, community, "1.3.6.1.4.1.1916.1.2.1.2.1.2", token, repetitions: maxRepetitions);
    // Выполняем SNMP-запрос для получения VlanTag
    vlansTags = await snmpCommandExecutor.WalkCommand(host, community, "1.3.6.1.4.1.1916.1.2.1.2.1.10", token, repetitions: maxRepetitions);
    // Выполняем SNMP-запрос для получения Base Port & VLAN Egress Ports
    portsToVlans = await snmpCommandExecutor.WalkCommand(host, community, "1.3.6.1.4.1.1916.1.4.17.1.2", token, repetitions: maxRepetitions);

    // Проверяем, что хотя бы запрос не вернул пустые данные
    if (portsToVlans.Count == 0) return;

    // Объединяем результаты SNMP-запросов
    var vlanTable = vlansNumbers.Zip(vlansNames, (vlanNumber, vlanName) => new { vlanNumber, vlanName })
                                .Zip(vlansTags, (firstPair, vlanTag) => new { VlanNumber = int.Parse(firstPair.vlanNumber.Data), VlanName = firstPair.vlanName.Data, VlanTag = int.Parse(vlanTag.Data) })
                                .ToDictionary(x => x.VlanNumber);

    var egressPorts = portsToVlans.Select(response =>
                                          {
                                            var oidParts = response.OID.Split('.');
                                            if (!int.TryParse(oidParts[^1], out int vlanNumber)) throw new FormatException("Unable to parse port index from OID.");
                                            if (!int.TryParse(oidParts[^2], out int portIfIndex)) throw new FormatException("Unable to parse port index from OID.");
                                            return new { PortIfIndex = portIfIndex, VlanNumber = vlanNumber };
                                          })
                                  .ToList();

    // Инициализируем словарь для быстрого поиска портов по их InterfaceNumber
    var portsDictionary = networkDevice.PortsOfNetworkDevice.ToDictionary(port => port.InterfaceNumber);

    var vlansToAdd = new HashSet<VLAN>();

    // Проходим по результатам vlanEntries
    foreach (var egressPort in egressPorts)
    {
      if (vlanTable.TryGetValue(egressPort.VlanNumber, out var vlan))
      {
        VLAN vlanToCreate = new() { Id = Guid.NewGuid(), VLANTag = vlan.VlanTag, VLANName = vlan.VlanName };
        // Ищем порт в коллекции PortsOfNetworkDevice по InterfaceNumber
        if (portsDictionary.TryGetValue(egressPort.PortIfIndex, out var matchingPort))
        {
          vlansToAdd.Add(vlanToCreate);
          // Если коллекция VLANs не инициализирована, инициализируем её
          matchingPort.VLANs ??= [];
          // Добавляем новый VLAN в порт
          matchingPort.VLANs.Add(vlanToCreate);
        }
      }
    }
  }

  class InterfaceData
  {
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public PortType Type { get; set; }
    public long Speed { get; set; }
    public PortStatus Status { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
  }

  class ArpEntry
  {
    public int InterfaceNumber { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
  }

  class NetworkEntry
  {
    public int InterfaceNumber { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string SubnetMask { get; set; } = string.Empty;
  }

  class VlanTableEntry
  {
    public int VlanTag { get; set; }
    public string VlanName { get; set; } = string.Empty;
    public IEnumerable<int> EgressPorts { get; set; } = [];
  }
}
