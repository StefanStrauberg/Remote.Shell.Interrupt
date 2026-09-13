using MediatR;
using Microsoft.Extensions.Configuration;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.SNMPRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.CreateNetworkDevice;
using Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;
using Remote.Shell.Interrupt.Storehouse.Domain.SNMP;

namespace Tests.Application.Features.NetworkDevices;

public class CreateNetworkDeviceCommandHandlerTests
{
    const string SystemDescriptionOid = "1.3.6.1.2.1.1.1.0";
    const string SystemNameOid = "1.3.6.1.2.1.1.5.0";
    const string InterfaceIndexOid = "1.3.6.1.2.1.2.2.1.1";
    const string InterfaceNameOid = "1.3.6.1.2.1.2.2.1.2";
    const string InterfaceTypeOid = "1.3.6.1.2.1.2.2.1.3";
    const string InterfaceSpeedOid = "1.3.6.1.2.1.2.2.1.5";
    const string InterfaceMacOid = "1.3.6.1.2.1.2.2.1.6";
    const string InterfaceStatusOid = "1.3.6.1.2.1.2.2.1.8";
    const string InterfaceDescriptionOid = "1.3.6.1.2.1.31.1.1.1.18";
    const string ArpIfIndexOid = "1.3.6.1.2.1.4.22.1.1";
    const string ArpMacOid = "1.3.6.1.2.1.4.22.1.2";
    const string ArpIpOid = "1.3.6.1.2.1.4.22.1.3";
    const string MacToVirtualPortOid = "1.3.6.1.2.1.17.4.3.1.2";
    const string Dot1dBasePortOid = "1.3.6.1.2.1.17.1.4.1.1";
    const string Dot1dBasePortIfIndexOid = "1.3.6.1.2.1.17.1.4.1.2";
    const string IpAddressIfIndexOid = "1.3.6.1.2.1.4.20.1.2";
    const string IpAddressOid = "1.3.6.1.2.1.4.20.1.1";
    const string SubnetMaskOid = "1.3.6.1.2.1.4.20.1.3";
    const string VlanStaticNameOid = "1.3.6.1.2.1.17.7.1.4.3.1.1";
    const string JuniperEgressOid = "1.3.6.1.2.1.17.7.1.4.3.1.2";
    const string HuaweiEgressOid = "1.3.6.1.2.1.17.7.1.4.2.1.4.0";
    const string IfStackOid = "1.3.6.1.2.1.31.1.2.1.3";
    const string HuaweiIfStackOid = "1.2.840.10006.300.43.1.1.2.1.1";
    const string ExtremeIfStackOid = "1.3.6.1.4.1.1916.1.4.3.1.4";
    const string ExtremeVlanNumberOid = "1.3.6.1.4.1.1916.1.2.1.2.1.1";
    const string ExtremeVlanNameOid = "1.3.6.1.4.1.1916.1.2.1.2.1.2";
    const string ExtremeVlanTagOid = "1.3.6.1.4.1.1916.1.2.1.2.1.10";
    const string ExtremePortsToVlansOid = "1.3.6.1.4.1.1916.1.4.17.1.2";

    readonly ISNMPCommandExecutor _executor = Substitute.For<ISNMPCommandExecutor>();
    readonly INetDevUnitOfWork _unitOfWork = Substitute.For<INetDevUnitOfWork>();
    readonly INetworkDeviceRepository _devices = Substitute.For<INetworkDeviceRepository>();
    readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    readonly List<NetworkDevice> _inserted = [];

    CreateNetworkDeviceCommandHandler CreateHandler()
    {
        _unitOfWork.NetworkDevices.Returns(_devices);
        _devices.InsertOne(Arg.Do<NetworkDevice>(d => _inserted.Add(d)));
        return new CreateNetworkDeviceCommandHandler(_executor, _unitOfWork, _configuration);
    }

    void SetupRepetitions(string key, string value)
    {
        var section = Substitute.For<IConfigurationSection>();
        section.Value.Returns(value);
        _configuration.GetSection(key).Returns(section);
    }

    void SetupSystemQueries()
    {
        _executor.GetCommand(Arg.Any<string>(), Arg.Any<string>(), SystemDescriptionOid,
                             Arg.Any<CancellationToken>(), Arg.Any<bool>())
                 .Returns(new SNMPResponse { OID = SystemDescriptionOid, Data = "TestOS" });
        _executor.GetCommand(Arg.Any<string>(), Arg.Any<string>(), SystemNameOid,
                             Arg.Any<CancellationToken>(), Arg.Any<bool>())
                 .Returns(new SNMPResponse { OID = SystemNameOid, Data = "gw-test" });
    }

    void SetupWalk(string oid, params SNMPResponse[] responses)
        => _executor.WalkCommand(Arg.Any<string>(), Arg.Any<string>(), oid,
                                 Arg.Any<CancellationToken>(), Arg.Any<bool>(), Arg.Any<int>())
                    .Returns([.. responses]);

    static SNMPResponse Resp(string oid, string data) => new() { OID = oid, Data = data };

    async Task<NetworkDevice> RunAsync(string host, string community, string deviceType)
    {
        var handler = CreateHandler();
        await ((ICommandHandler<CreateNetworkDeviceCommand, Unit>)handler)
            .Handle(new CreateNetworkDeviceCommand(host, community, deviceType), CancellationToken.None);
        return _inserted.Should().ContainSingle().Subject;
    }

    [Fact]
    public async Task Handle_UnknownDeviceType_ThrowsBadRequestWithoutSnmpCalls()
    {
        var handler = CreateHandler();
        SetupRepetitions("Repetitions:Default", "5");

        var act = async () => await ((ICommandHandler<CreateNetworkDeviceCommand, Unit>)handler)
            .Handle(new CreateNetworkDeviceCommand("10.0.0.1", "public", "NotAVendor"), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>()
                 .WithMessage("*NotAVendor*");
        await _executor.DidNotReceiveWithAnyArgs().GetCommand(default!, default!, default!, default, default);
    }

    [Fact]
    public async Task Handle_JuniperDevice_BuildsCompleteDeviceGraph()
    {
        SetupRepetitions("Repetitions:Juniper", "50");
        SetupSystemQueries();

        // 4 interfaces: two member ports, one LAG, one logical
        SetupWalk(InterfaceIndexOid,
                  Resp($"{InterfaceIndexOid}.1", "1"),
                  Resp($"{InterfaceIndexOid}.2", "2"),
                  Resp($"{InterfaceIndexOid}.3", "3"),
                  Resp($"{InterfaceIndexOid}.4", "4"));
        SetupWalk(InterfaceNameOid,
                  Resp($"{InterfaceNameOid}.1", "xe-0/0/0"),
                  Resp($"{InterfaceNameOid}.2", "xe-0/0/1"),
                  Resp($"{InterfaceNameOid}.3", "ae0"),
                  Resp($"{InterfaceNameOid}.4", "irb"));
        SetupWalk(InterfaceTypeOid,
                  Resp($"{InterfaceTypeOid}.1", "6"),
                  Resp($"{InterfaceTypeOid}.2", "6"),
                  Resp($"{InterfaceTypeOid}.3", "161"),
                  Resp($"{InterfaceTypeOid}.4", "24"));
        SetupWalk(InterfaceSpeedOid,
                  Resp($"{InterfaceSpeedOid}.1", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.2", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.3", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.4", "0"));
        SetupWalk(InterfaceMacOid,
                  Resp($"{InterfaceMacOid}.1", "AA BB CC DD EE 01"),
                  Resp($"{InterfaceMacOid}.2", "AA BB CC DD EE 02"),
                  Resp($"{InterfaceMacOid}.3", "AA BB CC DD EE 03"),
                  Resp($"{InterfaceMacOid}.4", ""));
        SetupWalk(InterfaceStatusOid,
                  Resp($"{InterfaceStatusOid}.1", "1"),
                  Resp($"{InterfaceStatusOid}.2", "1"),
                  Resp($"{InterfaceStatusOid}.3", "1"),
                  Resp($"{InterfaceStatusOid}.4", "2"));
        SetupWalk(InterfaceDescriptionOid,
                  Resp($"{InterfaceDescriptionOid}.1", "member0"),
                  Resp($"{InterfaceDescriptionOid}.2", "member1"),
                  Resp($"{InterfaceDescriptionOid}.3", "lag"),
                  Resp($"{InterfaceDescriptionOid}.4", "logical"));

        // ARP table: two entries on the first member port
        SetupWalk(ArpIfIndexOid,
                  Resp($"{ArpIfIndexOid}.1", "1"),
                  Resp($"{ArpIfIndexOid}.2", "1"));
        SetupWalk(ArpMacOid,
                  Resp($"{ArpMacOid}.1", "00 1A 2B 3C 4D 5E"),
                  Resp($"{ArpMacOid}.2", "00 1A 2B 3C 4D 5F"));
        SetupWalk(ArpIpOid,
                  Resp($"{ArpIpOid}.1", "192.168.1.10"),
                  Resp($"{ArpIpOid}.2", "192.168.1.11"));

        // MAC table: virtual port 5 → ifIndex 1, virtual port 6 → ifIndex 2
        SetupWalk(MacToVirtualPortOid,
                  Resp($"{MacToVirtualPortOid}.0.26.43.60.77.94", "5"),
                  Resp($"{MacToVirtualPortOid}.0.26.43.60.77.95", "5"),
                  Resp($"{MacToVirtualPortOid}.0.26.43.60.77.96", "6"));
        SetupWalk(Dot1dBasePortOid,
                  Resp($"{Dot1dBasePortOid}.5", "5"),
                  Resp($"{Dot1dBasePortOid}.6", "6"));
        SetupWalk(Dot1dBasePortIfIndexOid,
                  Resp($"{Dot1dBasePortIfIndexOid}.5", "1"),
                  Resp($"{Dot1dBasePortIfIndexOid}.6", "2"));

        // Interface addresses: ae0 (ifIndex 3) terminates 10.0.0.1/24
        SetupWalk(IpAddressIfIndexOid, Resp($"{IpAddressIfIndexOid}.3", "3"));
        SetupWalk(IpAddressOid, Resp($"{IpAddressOid}.3", "10.0.0.1"));
        SetupWalk(SubnetMaskOid, Resp($"{SubnetMaskOid}.3", "255.255.255.0"));

        // VLAN 100 egresses bridge ports 5 and 6
        SetupWalk(VlanStaticNameOid, Resp($"{VlanStaticNameOid}.100", "VLAN100"));
        SetupWalk(JuniperEgressOid, Resp($"{JuniperEgressOid}.100", "5, 6"));

        // ifStack: ae0 (ifIndex 3) aggregates xe-0/0/0 (ifIndex 1)
        SetupWalk(IfStackOid, Resp($"{IfStackOid}.3.1", "1"));

        var device = await RunAsync("10.0.0.1", "public", "Juniper");

        device.NetworkDeviceName.Should().Be("gw-test");
        device.GeneralInformation.Should().Be("TestOS");
        device.Host.Should().Be(ConvertStringIPAddressToLong.Handle("10.0.0.1"));
        device.TypeOfNetworkDevice.Should().Be(TypeOfNetworkDevice.Juniper);
        device.PortsOfNetworkDevice.Should().HaveCount(4);

        var xe0 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "xe-0/0/0");
        var ae0 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "ae0");
        var irb = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "irb");

        xe0.InterfaceType.Should().Be(PortType.ethernetCsmacd);
        ae0.InterfaceType.Should().Be(PortType.ieee8023adLag);
        irb.InterfaceType.Should().Be(PortType.softwareLoopback);
        irb.InterfaceStatus.Should().Be(PortStatus.down);

        xe0.MACAddress.Should().Be("AA:BB:CC:DD:EE:01");

        var arpTable = xe0.ARPTableOfInterface;
        arpTable.Should().HaveCount(2);
        arpTable.Should().Contain(a => a.MAC == "00:1A:2B:3C:4D:5E" && a.IPAddress == "192.168.1.10");
        arpTable.Should().Contain(a => a.MAC == "00:1A:2B:3C:4D:5F" && a.IPAddress == "192.168.1.11");
        arpTable.Should().OnlyContain(a => a.PortId == xe0.Id);

        xe0.MACTable.Should().HaveCount(2);
        device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "xe-0/0/1").MACTable.Should().HaveCount(1);

        xe0.VLANs.Should().ContainSingle(v => v.VLANTag == 100 && v.VLANName == "VLAN100");

        ae0.AggregatedPorts.Should().Contain(xe0);
        xe0.ParentId.Should().Be(ae0.Id);
        ae0.AggregatedPorts.Should().NotContain(device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "xe-0/0/1"));

        _unitOfWork.Received().Complete();
        _devices.Received().InsertOne(device);
    }

    [Fact]
    public async Task Handle_HuaweiDevice_AssignsVlansAndLinksAggregationFromHexTables()
    {
        SetupRepetitions("Repetitions:Huawei", "15");
        SetupSystemQueries();

        SetupWalk(InterfaceIndexOid,
                  Resp($"{InterfaceIndexOid}.6", "6"),
                  Resp($"{InterfaceIndexOid}.7", "7"),
                  Resp($"{InterfaceIndexOid}.52", "52"));
        SetupWalk(InterfaceNameOid,
                  Resp($"{InterfaceNameOid}.6", "GigabitEthernet0/0/0"),
                  Resp($"{InterfaceNameOid}.7", "GigabitEthernet0/0/1"),
                  Resp($"{InterfaceNameOid}.52", "Eth-Trunk1"));
        SetupWalk(InterfaceTypeOid,
                  Resp($"{InterfaceTypeOid}.6", "6"),
                  Resp($"{InterfaceTypeOid}.7", "6"),
                  Resp($"{InterfaceTypeOid}.52", "161"));
        SetupWalk(InterfaceSpeedOid,
                  Resp($"{InterfaceSpeedOid}.6", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.7", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.52", "0"));
        SetupWalk(InterfaceMacOid,
                  Resp($"{InterfaceMacOid}.6", "AA BB CC DD EE 06"),
                  Resp($"{InterfaceMacOid}.7", "AA BB CC DD EE 07"),
                  Resp($"{InterfaceMacOid}.52", ""));
        SetupWalk(InterfaceStatusOid,
                  Resp($"{InterfaceStatusOid}.6", "1"),
                  Resp($"{InterfaceStatusOid}.7", "1"),
                  Resp($"{InterfaceStatusOid}.52", "1"));
        SetupWalk(InterfaceDescriptionOid,
                  Resp($"{InterfaceDescriptionOid}.6", "g0"),
                  Resp($"{InterfaceDescriptionOid}.7", "g1"),
                  Resp($"{InterfaceDescriptionOid}.52", "trunk"));

        SetupWalk(ArpIfIndexOid, Resp($"{ArpIfIndexOid}.6", "6"));
        SetupWalk(ArpMacOid, Resp($"{ArpMacOid}.6", "00 1A 2B 3C 4D 60"));
        SetupWalk(ArpIpOid, Resp($"{ArpIpOid}.6", "192.168.2.6"));

        SetupWalk(MacToVirtualPortOid,
                  Resp($"{MacToVirtualPortOid}.0.26.43.60.77.94", "0"),
                  Resp($"{MacToVirtualPortOid}.0.26.43.60.77.95", "1"));
        SetupWalk(Dot1dBasePortOid,
                  Resp($"{Dot1dBasePortOid}.0", "0"),
                  Resp($"{Dot1dBasePortOid}.1", "1"));
        SetupWalk(Dot1dBasePortIfIndexOid,
                  Resp($"{Dot1dBasePortIfIndexOid}.0", "6"),
                  Resp($"{Dot1dBasePortIfIndexOid}.1", "7"));

        // Interface address: Eth-Trunk1 (ifIndex 52) terminates 10.0.1.1/24
        SetupWalk(IpAddressIfIndexOid, Resp($"{IpAddressIfIndexOid}.52", "52"));
        SetupWalk(IpAddressOid, Resp($"{IpAddressOid}.52", "10.0.1.1"));
        SetupWalk(SubnetMaskOid, Resp($"{SubnetMaskOid}.52", "255.255.255.0"));

        SetupWalk(VlanStaticNameOid, Resp($"{VlanStaticNameOid}.100", "VLAN100"));
        SetupWalk(HuaweiEgressOid, Resp($"{HuaweiEgressOid}.100", "C0"));

        // Standard ifStack empty → Huawei private if-stack table is queried.
        SetupWalk(IfStackOid);
        SetupWalk(HuaweiIfStackOid, Resp($"{HuaweiIfStackOid}.52", "C0"));

        var device = await RunAsync("10.0.1.1", "public", "Huawei");

        device.PortsOfNetworkDevice.Should().HaveCount(3);

        var g0 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "GigabitEthernet0/0/0");
        var g1 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "GigabitEthernet0/0/1");
        var trunk = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "Eth-Trunk1");

        g0.VLANs.Should().ContainSingle(v => v.VLANTag == 100);
        g1.VLANs.Should().ContainSingle(v => v.VLANTag == 100);

        g0.ARPTableOfInterface.Should().ContainSingle(a => a.IPAddress == "192.168.2.6");
        g0.MACTable.Should().ContainSingle(m => m.MACAddress == "00:1A:2B:3C:4D:5E");

        trunk.AggregatedPorts.Should().BeEquivalentTo([g0, g1]);
        g0.ParentId.Should().Be(trunk.Id);
        g1.ParentId.Should().Be(trunk.Id);
    }

    [Fact]
    public async Task Handle_ExtremeDevice_AssignsVlansFromOidTableAndRemovesManagementPorts()
    {
        SetupRepetitions("Repetitions:Extreme", "15");
        SetupSystemQueries();

        SetupWalk(InterfaceIndexOid,
                  Resp($"{InterfaceIndexOid}.1", "1"),
                  Resp($"{InterfaceIndexOid}.2", "2"),
                  Resp($"{InterfaceIndexOid}.3", "3"));
        SetupWalk(InterfaceNameOid,
                  Resp($"{InterfaceNameOid}.1", "1"),
                  Resp($"{InterfaceNameOid}.2", "2"),
                  Resp($"{InterfaceNameOid}.3", "Management"));
        SetupWalk(InterfaceTypeOid,
                  Resp($"{InterfaceTypeOid}.1", "6"),
                  Resp($"{InterfaceTypeOid}.2", "6"),
                  Resp($"{InterfaceTypeOid}.3", "6"));
        SetupWalk(InterfaceSpeedOid,
                  Resp($"{InterfaceSpeedOid}.1", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.2", "1000000000"),
                  Resp($"{InterfaceSpeedOid}.3", "0"));
        SetupWalk(InterfaceMacOid,
                  Resp($"{InterfaceMacOid}.1", "AA BB CC DD EE 11"),
                  Resp($"{InterfaceMacOid}.2", "AA BB CC DD EE 12"),
                  Resp($"{InterfaceMacOid}.3", ""));
        SetupWalk(InterfaceStatusOid,
                  Resp($"{InterfaceStatusOid}.1", "1"),
                  Resp($"{InterfaceStatusOid}.2", "1"),
                  Resp($"{InterfaceStatusOid}.3", "1"));
        SetupWalk(InterfaceDescriptionOid,
                  Resp($"{InterfaceDescriptionOid}.1", "p1"),
                  Resp($"{InterfaceDescriptionOid}.2", "p2"),
                  Resp($"{InterfaceDescriptionOid}.3", "mgmt"));

        SetupWalk(ArpIfIndexOid, Resp($"{ArpIfIndexOid}.2", "2"));
        SetupWalk(ArpMacOid, Resp($"{ArpMacOid}.2", "00 1A 2B 3C 4D 70"));
        SetupWalk(ArpIpOid, Resp($"{ArpIpOid}.2", "192.168.3.2"));

        SetupWalk(MacToVirtualPortOid, Resp($"{MacToVirtualPortOid}.0.26.43.60.77.94", "0"));
        SetupWalk(Dot1dBasePortOid, Resp($"{Dot1dBasePortOid}.0", "0"));
        SetupWalk(Dot1dBasePortIfIndexOid, Resp($"{Dot1dBasePortIfIndexOid}.0", "1"));

        // Interface address: port 2 terminates 10.0.2.1/24
        SetupWalk(IpAddressIfIndexOid, Resp($"{IpAddressIfIndexOid}.2", "2"));
        SetupWalk(IpAddressOid, Resp($"{IpAddressOid}.2", "10.0.2.1"));
        SetupWalk(SubnetMaskOid, Resp($"{SubnetMaskOid}.2", "255.255.255.0"));

        SetupWalk(ExtremeVlanNumberOid, Resp($"{ExtremeVlanNumberOid}.100", "100"));
        SetupWalk(ExtremeVlanNameOid, Resp($"{ExtremeVlanNameOid}.100", "CLIENTS"));
        SetupWalk(ExtremeVlanTagOid, Resp($"{ExtremeVlanTagOid}.100", "100"));
        SetupWalk(ExtremePortsToVlansOid, Resp($"{ExtremePortsToVlansOid}.1.100", "1"));

        SetupWalk(ExtremeIfStackOid, Resp($"{ExtremeIfStackOid}.2.1", "1"));

        var device = await RunAsync("10.0.2.1", "public", "Extreme");

        // Management port is filtered out.
        device.PortsOfNetworkDevice.Should().HaveCount(2);

        var port1 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "1");
        var port2 = device.PortsOfNetworkDevice.Single(p => p.InterfaceName == "2");

        port1.VLANs.Should().ContainSingle(v => v.VLANTag == 100 && v.VLANName == "CLIENTS");
        port1.MACTable.Should().ContainSingle();
        port2.ARPTableOfInterface.Should().ContainSingle(a => a.IPAddress == "192.168.3.2");

        port2.AggregatedPorts.Should().Contain(port1);
        port1.ParentId.Should().Be(port2.Id);
    }

    [Fact]
    public async Task Handle_ArpCountMismatch_ThrowsInvalidOperationException()
    {
        SetupRepetitions("Repetitions:Default", "5");
        SetupSystemQueries();

        SetupWalk(InterfaceIndexOid, Resp($"{InterfaceIndexOid}.1", "1"));
        SetupWalk(InterfaceNameOid, Resp($"{InterfaceNameOid}.1", "xe-0/0/0"));
        SetupWalk(InterfaceTypeOid, Resp($"{InterfaceTypeOid}.1", "6"));
        SetupWalk(InterfaceSpeedOid, Resp($"{InterfaceSpeedOid}.1", "1"));
        SetupWalk(InterfaceMacOid, Resp($"{InterfaceMacOid}.1", "AA BB CC DD EE 01"));
        SetupWalk(InterfaceStatusOid, Resp($"{InterfaceStatusOid}.1", "1"));
        SetupWalk(InterfaceDescriptionOid, Resp($"{InterfaceDescriptionOid}.1", "d"));

        // Inconsistent ARP lists: one ifIndex, but two MACs.
        SetupWalk(ArpIfIndexOid, Resp($"{ArpIfIndexOid}.1", "1"));
        SetupWalk(ArpMacOid,
                  Resp($"{ArpMacOid}.1", "00 1A 2B 3C 4D 5E"),
                  Resp($"{ArpMacOid}.2", "00 1A 2B 3C 4D 5F"));
        SetupWalk(ArpIpOid,
                  Resp($"{ArpIpOid}.1", "192.168.1.10"),
                  Resp($"{ArpIpOid}.2", "192.168.1.11"));

        var handler = CreateHandler();
        var act = async () => await ((ICommandHandler<CreateNetworkDeviceCommand, Unit>)handler)
            .Handle(new CreateNetworkDeviceCommand("10.0.0.1", "public", "Cisco"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*SNMP responses count mismatch*");
        _inserted.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_EmptyArpResponse_ThrowsInvalidOperationException()
    {
        SetupRepetitions("Repetitions:Default", "5");
        SetupSystemQueries();

        SetupWalk(InterfaceIndexOid, Resp($"{InterfaceIndexOid}.1", "1"));
        SetupWalk(InterfaceNameOid, Resp($"{InterfaceNameOid}.1", "xe-0/0/0"));
        SetupWalk(InterfaceTypeOid, Resp($"{InterfaceTypeOid}.1", "6"));
        SetupWalk(InterfaceSpeedOid, Resp($"{InterfaceSpeedOid}.1", "1"));
        SetupWalk(InterfaceMacOid, Resp($"{InterfaceMacOid}.1", "AA BB CC DD EE 01"));
        SetupWalk(InterfaceStatusOid, Resp($"{InterfaceStatusOid}.1", "1"));
        SetupWalk(InterfaceDescriptionOid, Resp($"{InterfaceDescriptionOid}.1", "d"));

        SetupWalk(ArpIfIndexOid);
        SetupWalk(ArpMacOid);
        SetupWalk(ArpIpOid);

        var handler = CreateHandler();
        var act = async () => await ((ICommandHandler<CreateNetworkDeviceCommand, Unit>)handler)
            .Handle(new CreateNetworkDeviceCommand("10.0.0.1", "public", "Cisco"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*empty results*");
    }
}
