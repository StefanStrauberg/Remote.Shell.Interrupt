namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Seed;

/// <summary>
/// Builds the "Network device discovery" workflow that replaces
/// CreateNetworkDeviceCommandHandler's hand-coded vendor branching (Juniper/Huawei/Extreme):
/// general info -> ports -> ARP/MAC/network tables -> per-vendor VLAN assignment, port
/// filtering and link aggregation -> save. Seeded once at startup by WorkflowSeeder under
/// <see cref="WorkflowId"/> - see that class for why a fixed Id and Draft status.
/// </summary>
public static class DefaultNetworkDeviceWorkflowFactory
{
  public static readonly Guid WorkflowId = Guid.Parse("00000000-0000-0000-0000-000000000001");

  // SNMP OIDs, unchanged from CreateNetworkDeviceCommandHandler.
  const string SystemDescriptionOid = "1.3.6.1.2.1.1.1.0",
               SystemNameOid = "1.3.6.1.2.1.1.5.0",
               InterfaceIndexOid = "1.3.6.1.2.1.2.2.1.1",
               InterfaceNameOid = "1.3.6.1.2.1.2.2.1.2",
               InterfaceTypeOid = "1.3.6.1.2.1.2.2.1.3",
               InterfaceSpeedOid = "1.3.6.1.2.1.2.2.1.5",
               InterfaceMacOid = "1.3.6.1.2.1.2.2.1.6",
               InterfaceStatusOid = "1.3.6.1.2.1.2.2.1.8",
               InterfaceDescriptionOid = "1.3.6.1.2.1.31.1.1.1.18",
               ArpIfIndexOid = "1.3.6.1.2.1.4.22.1.1",
               ArpMacOid = "1.3.6.1.2.1.4.22.1.2",
               ArpIpOid = "1.3.6.1.2.1.4.22.1.3",
               MacToVirtualPortOid = "1.3.6.1.2.1.17.4.3.1.2",
               Dot1dBasePortOid = "1.3.6.1.2.1.17.1.4.1.1",
               Dot1dBasePortIfIndexOid = "1.3.6.1.2.1.17.1.4.1.2",
               IpAddressIfIndexOid = "1.3.6.1.2.1.4.20.1.2",
               IpAddressOid = "1.3.6.1.2.1.4.20.1.1",
               SubnetMaskOid = "1.3.6.1.2.1.4.20.1.3",
               VlanStaticNameOid = "1.3.6.1.2.1.17.7.1.4.3.1.1",
               JuniperEgressOid = "1.3.6.1.2.1.17.7.1.4.3.1.2",
               HuaweiEgressOid = "1.3.6.1.2.1.17.7.1.4.2.1.4.0",
               IfStackOid = "1.3.6.1.2.1.31.1.2.1.3",
               HuaweiIfStackOid = "1.2.840.10006.300.43.1.1.2.1.1",
               ExtremeIfStackOid = "1.3.6.1.4.1.1916.1.4.3.1.4",
               ExtremeVlanNumberOid = "1.3.6.1.4.1.1916.1.2.1.2.1.1",
               ExtremeVlanNameOid = "1.3.6.1.4.1.1916.1.2.1.2.1.2",
               ExtremeVlanTagOid = "1.3.6.1.4.1.1916.1.2.1.2.1.10",
               ExtremePortsToVlansOid = "1.3.6.1.4.1.1916.1.4.17.1.2";

  // Repetition counts carried over from appsettings.json's old Repetitions:* section.
  // Vendor-independent walks (general/ports/ARP/MAC/network-table) use the largest value
  // (Juniper's 50) so they're never truncated on a larger device - vendor isn't known yet at
  // that point in the graph. Using a larger-than-needed value only costs a slightly bigger
  // GETBULK batch, never incorrect (truncated) results.
  const int SafeDefaultRepetitions = 50, HuaweiRepetitions = 15, ExtremeRepetitions = 15;

  public static WorkflowDefinition Create()
  {
    // ---- Phase 0-1: Start, general info ----
    var start = Node("Start", "Start");

    var getSysDescr = Node("SnmpGet", "Read sysDescr", "get-sysdescr", new()
    {
      ["oid"] = SystemDescriptionOid,
      ["output"] = "device.generalInformation"
    });

    var getSysName = Node("SnmpGet", "Read sysName", "get-sysname", new()
    {
      ["oid"] = SystemNameOid,
      ["output"] = "device.name"
    });

    // ---- Phase 2: ports ----
    var getIfNumber = Walk("Read ifIndex", InterfaceIndexOid, "raw.if.number");
    var getIfName = Walk("Read ifName", InterfaceNameOid, "raw.if.name");
    var getIfType = Walk("Read ifType", InterfaceTypeOid, "raw.if.type");
    var getIfSpeed = Walk("Read ifSpeed", InterfaceSpeedOid, "raw.if.speed");
    var getIfMac = Walk("Read ifPhysAddress", InterfaceMacOid, "raw.if.mac", toHex: true);
    var getIfStatus = Walk("Read ifOperStatus", InterfaceStatusOid, "raw.if.status");
    var getIfDescription = Walk("Read ifAlias", InterfaceDescriptionOid, "raw.if.description");

    var combinePorts = Script("Combine port data", "combine-ports", output: "network.ports", source: """
      function execute(input, context) {
        var numbers = context['raw.if.number'];
        var names = context['raw.if.name'];
        var types = context['raw.if.type'];
        var speeds = context['raw.if.speed'];
        var macs = context['raw.if.mac'];
        var statuses = context['raw.if.status'];
        var descriptions = context['raw.if.description'];

        var ports = [];
        for (var i = 0; i < numbers.length; i++) {
          ports.push({
            number: parseInt(numbers[i].data, 10),
            name: names[i].data,
            type: types[i].data,
            speed: parseInt(speeds[i].data, 10),
            mac: formatMac(macs[i].data),
            status: statuses[i].data,
            description: descriptions[i].data,
            vlans: [],
            macTable: [],
            arp: [],
            network: [],
            aggregatedPortNumbers: []
          });
        }
        return ports;
      }
      """);

    // ---- Phase 3: ARP table ----
    var getArpIfIndex = Walk("Read ipNetToMediaIfIndex", ArpIfIndexOid, "raw.arp.ifIndex");
    var getArpMac = Walk("Read ipNetToMediaPhysAddress", ArpMacOid, "raw.arp.mac", toHex: true);
    var getArpIp = Walk("Read ipNetToMediaNetAddress", ArpIpOid, "raw.arp.ip");

    var assignArp = Script("Assign ARP table", "assign-arp", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var ifIndexes = context['raw.arp.ifIndex'];
        var macs = context['raw.arp.mac'];
        var ips = context['raw.arp.ip'];

        if (ifIndexes.length === 0 || macs.length === 0 || ips.length === 0) {
          throw new Error('One or more SNMP requests returned empty results for ARP data.');
        }
        if (ifIndexes.length !== macs.length || macs.length !== ips.length) {
          throw new Error('SNMP responses count mismatch for ARP data: interfaces(' + ifIndexes.length +
            '), macs(' + macs.length + '), ips(' + ips.length + ')');
        }

        var byInterface = {};
        for (var i = 0; i < ifIndexes.length; i++) {
          var ifNum = parseInt(ifIndexes[i].data, 10);
          if (!byInterface[ifNum]) byInterface[ifNum] = [];
          byInterface[ifNum].push({ mac: formatMac(macs[i].data), ip: ips[i].data });
        }

        ports.forEach(function (port) {
          if (byInterface[port.number]) port.arp = byInterface[port.number];
        });

        return ports;
      }
      """);

    // ---- Phase 4: MAC table ----
    var getMacToVp = Walk("Read dot1dTpFdbPort", MacToVirtualPortOid, "raw.mac.macToVirtualPort");
    var getVpNumbers = Walk("Read dot1dBasePort", Dot1dBasePortOid, "raw.mac.virtualPorts");
    var getVpToIf = Walk("Read dot1dBasePortIfIndex", Dot1dBasePortIfIndexOid, "raw.mac.virtualPortToInterfaces");

    var assignMacTable = Script("Assign MAC table", "assign-mac-table", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var macToVp = context['raw.mac.macToVirtualPort'];
        var vpNumbers = context['raw.mac.virtualPorts'];
        var vpToIf = context['raw.mac.virtualPortToInterfaces'];

        if (macToVp.length === 0 || vpNumbers.length === 0 || vpToIf.length === 0) {
          return ports;
        }
        if (vpNumbers.length !== vpToIf.length) {
          throw new Error('SNMP responses count mismatch for virtual port data: virtual ports(' + vpNumbers.length +
            '), virtual port to interface(' + vpToIf.length + ')');
        }

        var macTable = {};
        macToVp.forEach(function (r) {
          var vp = parseInt(r.data, 10);
          if (!macTable[vp]) macTable[vp] = [];
          macTable[vp].push(macFromOid(r.oid));
        });

        var interfaceMapping = {};
        for (var i = 0; i < vpNumbers.length; i++) {
          interfaceMapping[parseInt(vpNumbers[i].data, 10)] = parseInt(vpToIf[i].data, 10);
        }

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        Object.keys(macTable).forEach(function (vpKey) {
          var ifNum = interfaceMapping[vpKey];
          if (ifNum !== undefined && portsByNumber[ifNum]) {
            macTable[vpKey].forEach(function (mac) {
              portsByNumber[ifNum].macTable.push(mac);
            });
          }
        });

        return ports;
      }
      """);

    // ---- Phase 5: network (terminated) table ----
    var getNetIfIndex = Walk("Read ipAdEntIfIndex", IpAddressIfIndexOid, "raw.net.ifIndex");
    var getNetIp = Walk("Read ipAdEntAddr", IpAddressOid, "raw.net.ip");
    var getNetMask = Walk("Read ipAdEntNetMask", SubnetMaskOid, "raw.net.mask");

    var assignNetworkTable = Script("Assign network table", "assign-network-table", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var ifIndexes = context['raw.net.ifIndex'];
        var ips = context['raw.net.ip'];
        var masks = context['raw.net.mask'];

        if (ifIndexes.length === 0 || ips.length === 0 || masks.length === 0) {
          throw new Error('One or more SNMP requests returned empty results for network table data.');
        }
        if (ifIndexes.length !== ips.length || ips.length !== masks.length) {
          throw new Error('SNMP responses count mismatch for network table data: interfaces(' + ifIndexes.length +
            '), IP addresses(' + ips.length + '), subnet masks(' + masks.length + ')');
        }

        var byInterface = {};
        for (var i = 0; i < ifIndexes.length; i++) {
          var ifNum = parseInt(ifIndexes[i].data, 10);
          if (!byInterface[ifNum]) byInterface[ifNum] = [];
          byInterface[ifNum].push({ ip: ips[i].data, mask: masks[i].data });
        }

        ports.forEach(function (port) {
          if (byInterface[port.number]) port.network = byInterface[port.number];
        });

        return ports;
      }
      """);

    // ---- Phase 6: vendor decision ----
    var chooseVendor = Node("Decision", "Choose vendor", "choose-vendor", new() { ["variable"] = "input.vendor" });

    // ---- Juniper branch ----
    var jGetBasePort = Walk("Juniper: read dot1dBasePort", Dot1dBasePortOid, "raw.vlan.basePort");
    var jGetPortIfIndex = Walk("Juniper: read dot1dBasePortIfIndex", Dot1dBasePortIfIndexOid, "raw.vlan.portIfIndex");
    var jGetVlanName = Walk("Juniper: read dot1qVlanStaticName", VlanStaticNameOid, "raw.vlan.name");
    var jGetVlanEgress = Walk("Juniper: read dot1qVlanStaticEgressPorts", JuniperEgressOid, "raw.vlan.egress");

    var jAssignVlans = Script("Juniper: assign VLANs", "juniper-assign-vlans", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var basePorts = context['raw.vlan.basePort'];
        var portIfIndexes = context['raw.vlan.portIfIndex'];
        var vlanNames = context['raw.vlan.name'];
        var vlanEgress = context['raw.vlan.egress'];

        if (basePorts.length === 0 || portIfIndexes.length === 0 || vlanNames.length === 0 || vlanEgress.length === 0) {
          throw new Error('One or more SNMP requests returned empty results for VLAN data.');
        }
        if (basePorts.length !== portIfIndexes.length) {
          throw new Error('SNMP responses count mismatch: dot1dBasePort(' + basePorts.length +
            ') != dot1dBasePortIfIndex(' + portIfIndexes.length + ')');
        }
        if (vlanNames.length !== vlanEgress.length) {
          throw new Error('SNMP responses count mismatch: dot1qVlanStaticName(' + vlanNames.length +
            ') != dot1qVlanStaticEgressPorts(' + vlanEgress.length + ')');
        }

        var physicalInterfaceTable = {};
        for (var i = 0; i < basePorts.length; i++) {
          physicalInterfaceTable[parseInt(basePorts[i].data, 10)] = parseInt(portIfIndexes[i].data, 10);
        }

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        for (var j = 0; j < vlanNames.length; j++) {
          var tag = oidLast(vlanNames[j].oid);
          var name = trimVlanNamePlusDigit(vlanNames[j].data);
          var egressPorts = parseJuniperEgress(vlanEgress[j].data);

          egressPorts.forEach(function (egressPort) {
            var ifIndex = physicalInterfaceTable[egressPort];
            if (ifIndex !== undefined && portsByNumber[ifIndex]) {
              portsByNumber[ifIndex].vlans.push({ tag: tag, name: name });
            }
          });
        }

        return ports;
      }
      """);

    var jKeepPorts = Script("Juniper: keep xe/irb/ae ports", "juniper-keep-ports", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        return ports.filter(function (p) {
          return p.name.startsWith('xe') || p.name.startsWith('irb') || p.name.startsWith('ae');
        });
      }
      """);

    var jGetIfStack = Walk("Juniper: read ifStackTable", IfStackOid, "raw.agg.ifStack");

    var jLinkAggregation = Script("Juniper: link aggregation", "juniper-link-aggregation", input: "network.ports", output: "network.ports", source: """
      function groupPortsByBaseInterface(ports) {
        var groups = [];
        var basePorts = ports.filter(function (p) { return p.name.indexOf('.') === -1; });

        basePorts.forEach(function (basePort) {
          var group = [basePort];
          var numbers = [basePort.number];
          ports.forEach(function (p) {
            if (p.name.indexOf(basePort.name + '.') === 0) {
              group.push(p);
              numbers.push(p.number);
            }
          });
          groups.push({ numbers: numbers, ports: group });
        });

        return groups;
      }

      function findPortInGroups(groups, number) {
        for (var i = 0; i < groups.length; i++) {
          for (var j = 0; j < groups[i].ports.length; j++) {
            if (groups[i].ports[j].number === number) return groups[i].ports[j];
          }
        }
        return null;
      }

      function execute(ports, context) {
        var ifStack = context['raw.agg.ifStack'];
        if (ifStack.length === 0) return ports;

        var aePorts = ports.filter(function (p) { return p.name.startsWith('ae'); });
        var xePorts = ports.filter(function (p) { return p.name.startsWith('xe'); });

        var aeGroups = groupPortsByBaseInterface(aePorts);
        var xeGroups = groupPortsByBaseInterface(xePorts);

        var aggregationNumbers = {};
        aeGroups.forEach(function (g) { g.numbers.forEach(function (n) { aggregationNumbers[n] = true; }); });

        ifStack.forEach(function (entry) {
          var aggNum = oidLastButOne(entry.oid);
          var memberNum = oidLast(entry.oid);
          if (aggNum === 0 || memberNum === 0 || !aggregationNumbers[aggNum]) return;

          var aggPort = findPortInGroups(aeGroups, aggNum);
          var memberPort = findPortInGroups(xeGroups, memberNum);
          if (aggPort && memberPort && aggPort.aggregatedPortNumbers.indexOf(memberPort.number) === -1) {
            aggPort.aggregatedPortNumbers.push(memberPort.number);
          }
        });

        return ports;
      }
      """);

    var jRemoveDotNotationPorts = Script("Juniper: remove dot-notation ports", "juniper-remove-dot-ports", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var aeXeBaseNames = {};
        ports.forEach(function (p) {
          if ((p.name.startsWith('ae') || p.name.startsWith('xe')) && p.name.indexOf('.') === -1) {
            aeXeBaseNames[p.name] = true;
          }
        });

        return ports.filter(function (p) {
          var dotIndex = p.name.indexOf('.');
          if (dotIndex === -1) return true;
          return !aeXeBaseNames[p.name.substring(0, dotIndex)];
        });
      }
      """);

    // ---- Huawei branch ----
    var hGetBasePort = Walk("Huawei: read dot1dBasePort", Dot1dBasePortOid, "raw.vlan.basePort", repetitions: HuaweiRepetitions);
    var hGetPortIfIndex = Walk("Huawei: read dot1dBasePortIfIndex", Dot1dBasePortIfIndexOid, "raw.vlan.portIfIndex", repetitions: HuaweiRepetitions);
    var hGetVlanName = Walk("Huawei: read dot1qVlanStaticName", VlanStaticNameOid, "raw.vlan.name", repetitions: HuaweiRepetitions);
    var hGetVlanEgress = Walk("Huawei: read dot1qVlanStaticEgressPorts", HuaweiEgressOid, "raw.vlan.egress", toHex: true, repetitions: HuaweiRepetitions);

    var hAssignVlans = Script("Huawei: assign VLANs", "huawei-assign-vlans", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var basePorts = context['raw.vlan.basePort'];
        var portIfIndexes = context['raw.vlan.portIfIndex'];
        var vlanNames = context['raw.vlan.name'];
        var vlanEgress = context['raw.vlan.egress'];

        if (basePorts.length === 0 || portIfIndexes.length === 0 || vlanNames.length === 0 || vlanEgress.length === 0) {
          throw new Error('One or more SNMP requests returned empty results for VLAN data.');
        }
        if (basePorts.length !== portIfIndexes.length) {
          throw new Error('SNMP responses count mismatch: dot1dBasePort(' + basePorts.length +
            ') != dot1dBasePortIfIndex(' + portIfIndexes.length + ')');
        }
        if (vlanNames.length !== vlanEgress.length) {
          throw new Error('SNMP responses count mismatch: dot1qVlanStaticName(' + vlanNames.length +
            ') != dot1qVlanStaticEgressPorts(' + vlanEgress.length + ')');
        }

        var physicalInterfaceTable = {};
        for (var i = 0; i < basePorts.length; i++) {
          physicalInterfaceTable[parseInt(basePorts[i].data, 10)] = parseInt(portIfIndexes[i].data, 10);
        }

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        for (var j = 0; j < vlanNames.length; j++) {
          var tag = oidLast(vlanNames[j].oid);
          var name = trimVlanNamePlusDigit(vlanNames[j].data);
          var egressPorts = parseHuaweiEgress(vlanEgress[j].data);

          egressPorts.forEach(function (egressPort) {
            var ifIndex = physicalInterfaceTable[egressPort];
            if (ifIndex !== undefined && portsByNumber[ifIndex]) {
              portsByNumber[ifIndex].vlans.push({ tag: tag, name: name });
            }
          });
        }

        return ports;
      }
      """);

    var hGetIfStackStandard = Walk("Huawei: read ifStackTable", IfStackOid, "raw.agg.ifStackStandard", repetitions: HuaweiRepetitions);
    var hGetIfStackPrivate = Walk("Huawei: read private ifStack table", HuaweiIfStackOid, "raw.agg.ifStackHuawei", toHex: true, repetitions: HuaweiRepetitions);

    var hLinkAggregation = Script("Huawei: link aggregation", "huawei-link-aggregation", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        // Matches the original handler's behavior exactly: a non-empty standard ifStack
        // table means Huawei aggregation is skipped entirely (not processed via the
        // standard table the way Juniper's is) - only an empty standard table triggers the
        // private-table fallback.
        if (context['raw.agg.ifStackStandard'].length > 0) return ports;

        var huaweiIfStack = context['raw.agg.ifStackHuawei'];
        if (huaweiIfStack.length === 0) return ports;

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        huaweiIfStack.forEach(function (entry) {
          var aggNum = oidLast(entry.oid);
          var memberNumbers = parseHuaweiEgress(entry.data);
          if (memberNumbers.length === 0) return;

          var aggPort = portsByNumber[aggNum];
          if (!aggPort) return;

          memberNumbers.forEach(function (portNumber) {
            var actualPortNumber = portNumber > 48 ? portNumber + 12 : portNumber + 6;
            var memberPort = portsByNumber[actualPortNumber];
            if (memberPort && aggPort.aggregatedPortNumbers.indexOf(memberPort.number) === -1) {
              aggPort.aggregatedPortNumbers.push(memberPort.number);
            }
          });
        });

        return ports;
      }
      """);

    var hRemovePorts = Script("Huawei: remove InLoop/MEth/NULL0 ports", "huawei-remove-ports", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        return ports.filter(function (p) {
          return !(p.name.startsWith('InLoop') || p.name.startsWith('MEth') || p.name.startsWith('NULL0'));
        });
      }
      """);

    // ---- Extreme branch ----
    var eGetVlanNumber = Walk("Extreme: read vlan number", ExtremeVlanNumberOid, "raw.vlan.number", repetitions: ExtremeRepetitions);
    var eGetVlanName = Walk("Extreme: read vlan name", ExtremeVlanNameOid, "raw.vlan.name", repetitions: ExtremeRepetitions);
    var eGetVlanTag = Walk("Extreme: read vlan tag", ExtremeVlanTagOid, "raw.vlan.tag", repetitions: ExtremeRepetitions);
    var eGetPortsToVlans = Walk("Extreme: read ports-to-VLANs", ExtremePortsToVlansOid, "raw.vlan.portsToVlans", repetitions: ExtremeRepetitions);

    var eAssignVlans = Script("Extreme: assign VLANs", "extreme-assign-vlans", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var vlanNumbers = context['raw.vlan.number'];
        var vlanNames = context['raw.vlan.name'];
        var vlanTags = context['raw.vlan.tag'];
        var portsToVlans = context['raw.vlan.portsToVlans'];

        if (portsToVlans.length === 0) return ports;

        var vlanTable = {};
        for (var i = 0; i < vlanNumbers.length; i++) {
          var num = parseInt(vlanNumbers[i].data, 10);
          vlanTable[num] = { tag: parseInt(vlanTags[i].data, 10), name: vlanNames[i].data };
        }

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        portsToVlans.forEach(function (entry) {
          var vlanNumber = oidLast(entry.oid);
          var portIfIndex = oidLastButOne(entry.oid);

          var vlanInfo = vlanTable[vlanNumber];
          if (vlanInfo && portsByNumber[portIfIndex]) {
            portsByNumber[portIfIndex].vlans.push({ tag: vlanInfo.tag, name: vlanInfo.name });
          }
        });

        return ports;
      }
      """);

    var eRemovePorts = Script("Extreme: remove Management/Virtual/VLAN ports", "extreme-remove-ports", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        return ports.filter(function (p) {
          return !(p.name.startsWith('Management') || p.name.startsWith('Virtual') || p.name.startsWith('VLAN'));
        });
      }
      """);

    var eGetIfStack = Walk("Extreme: read ifStackTable", ExtremeIfStackOid, "raw.agg.ifStackExtreme", repetitions: ExtremeRepetitions);

    var eLinkAggregation = Script("Extreme: link aggregation", "extreme-link-aggregation", input: "network.ports", output: "network.ports", source: """
      function execute(ports, context) {
        var ifStack = context['raw.agg.ifStackExtreme'];
        if (ifStack.length === 0) return ports;

        var portsByNumber = {};
        ports.forEach(function (p) { portsByNumber[p.number] = p; });

        ifStack.forEach(function (entry) {
          var aggNum = oidLastButOne(entry.oid);
          var memberNum = oidLast(entry.oid);
          if (aggNum === 0 || memberNum === 0 || aggNum === memberNum) return;

          var aggPort = portsByNumber[aggNum];
          var memberPort = portsByNumber[memberNum];
          if (aggPort && memberPort && aggPort.aggregatedPortNumbers.indexOf(memberPort.number) === -1) {
            aggPort.aggregatedPortNumbers.push(memberPort.number);
          }
        });

        return ports;
      }
      """);

    // ---- Convergence + save ----
    var join = Node("Join", "Join vendor branches");
    var save = Node("SaveNetworkDevice", "Save network device");
    var end = Node("End", "End");

    var nodes = new List<NodeDefinition>
    {
      start, getSysDescr, getSysName,
      getIfNumber, getIfName, getIfType, getIfSpeed, getIfMac, getIfStatus, getIfDescription, combinePorts,
      getArpIfIndex, getArpMac, getArpIp, assignArp,
      getMacToVp, getVpNumbers, getVpToIf, assignMacTable,
      getNetIfIndex, getNetIp, getNetMask, assignNetworkTable,
      chooseVendor,
      jGetBasePort, jGetPortIfIndex, jGetVlanName, jGetVlanEgress, jAssignVlans, jKeepPorts, jGetIfStack, jLinkAggregation, jRemoveDotNotationPorts,
      hGetBasePort, hGetPortIfIndex, hGetVlanName, hGetVlanEgress, hAssignVlans, hGetIfStackStandard, hGetIfStackPrivate, hLinkAggregation, hRemovePorts,
      eGetVlanNumber, eGetVlanName, eGetVlanTag, eGetPortsToVlans, eAssignVlans, eRemovePorts, eGetIfStack, eLinkAggregation,
      join, save, end
    };

    var edges = new List<EdgeDefinition>
    {
      Edge(start, getSysDescr), Edge(getSysDescr, getSysName), Edge(getSysName, getIfNumber),
      Edge(getIfNumber, getIfName), Edge(getIfName, getIfType), Edge(getIfType, getIfSpeed),
      Edge(getIfSpeed, getIfMac), Edge(getIfMac, getIfStatus), Edge(getIfStatus, getIfDescription),
      Edge(getIfDescription, combinePorts),

      Edge(combinePorts, getArpIfIndex), Edge(getArpIfIndex, getArpMac), Edge(getArpMac, getArpIp), Edge(getArpIp, assignArp),

      Edge(assignArp, getMacToVp), Edge(getMacToVp, getVpNumbers), Edge(getVpNumbers, getVpToIf), Edge(getVpToIf, assignMacTable),

      Edge(assignMacTable, getNetIfIndex), Edge(getNetIfIndex, getNetIp), Edge(getNetIp, getNetMask), Edge(getNetMask, assignNetworkTable),

      Edge(assignNetworkTable, chooseVendor),

      // Juniper: VLANs -> keep-list filter -> aggregation -> dot-notation removal -> Join
      Edge(chooseVendor, jGetBasePort, "Juniper", 1),
      Edge(jGetBasePort, jGetPortIfIndex), Edge(jGetPortIfIndex, jGetVlanName), Edge(jGetVlanName, jGetVlanEgress),
      Edge(jGetVlanEgress, jAssignVlans), Edge(jAssignVlans, jKeepPorts), Edge(jKeepPorts, jGetIfStack),
      Edge(jGetIfStack, jLinkAggregation), Edge(jLinkAggregation, jRemoveDotNotationPorts), Edge(jRemoveDotNotationPorts, join),

      // Huawei: VLANs -> aggregation -> filter -> Join
      Edge(chooseVendor, hGetBasePort, "Huawei", 2),
      Edge(hGetBasePort, hGetPortIfIndex), Edge(hGetPortIfIndex, hGetVlanName), Edge(hGetVlanName, hGetVlanEgress),
      Edge(hGetVlanEgress, hAssignVlans), Edge(hAssignVlans, hGetIfStackStandard), Edge(hGetIfStackStandard, hGetIfStackPrivate),
      Edge(hGetIfStackPrivate, hLinkAggregation), Edge(hLinkAggregation, hRemovePorts), Edge(hRemovePorts, join),

      // Extreme: VLANs -> filter -> aggregation -> Join
      Edge(chooseVendor, eGetVlanNumber, "Extreme", 3),
      Edge(eGetVlanNumber, eGetVlanName), Edge(eGetVlanName, eGetVlanTag), Edge(eGetVlanTag, eGetPortsToVlans),
      Edge(eGetPortsToVlans, eAssignVlans), Edge(eAssignVlans, eRemovePorts), Edge(eRemovePorts, eGetIfStack),
      Edge(eGetIfStack, eLinkAggregation), Edge(eLinkAggregation, join),

      // Any other vendor: straight through, unchanged (matches the original handler doing
      // nothing vendor-specific for anything besides Juniper/Huawei/Extreme).
      Edge(chooseVendor, join, "DEFAULT", 100),

      Edge(join, save), Edge(save, end)
    };

    return new WorkflowDefinition
    {
      Id = WorkflowId,
      Name = "Network device discovery",
      Version = 1,
      Status = WorkflowStatus.Draft,
      StartNodeId = start.Id,
      Nodes = nodes,
      Edges = edges
    };
  }

  static NodeDefinition Node(string type, string name, string? key = null, Dictionary<string, object?>? config = null)
    => new() { Id = Guid.NewGuid(), Type = type, Name = name, Key = key, Config = config ?? [] };

  static NodeDefinition Walk(string name, string oid, string output, bool toHex = false, int repetitions = SafeDefaultRepetitions)
    => Node("SnmpWalk", name, config: new()
    {
      ["oid"] = oid,
      ["output"] = output,
      ["toHex"] = toHex,
      ["repetitions"] = repetitions
    });

  static NodeDefinition Script(string name, string key, string source, string? input = null, string? output = null)
    => Node("Script", name, key, new()
    {
      ["scriptSource"] = source,
      ["input"] = input ?? string.Empty,
      ["output"] = output ?? $"script.{key}"
    });

  static EdgeDefinition Edge(NodeDefinition from, NodeDefinition to, string? condition = null, int priority = 0)
    => new() { Id = Guid.NewGuid(), FromNodeId = from.Id, ToNodeId = to.Id, Condition = condition, Priority = priority };
}
