using AutoMapper;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.ARPEntities;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Ports;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.TerminatedNetworkEntities;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.TfPlans;
using Remote.Shell.Interrupt.Storehouse.Application.Services.Mapping;

namespace Tests.Application.Mapping;

public class AssemblyMappingProfileTests
{
    static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AssemblyMappingProfile(typeof(GateDTO).Assembly)),
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public void GateToGateDto_ConvertsLongIpAndEnumToString()
    {
        var mapper = CreateMapper();
        var gate = new Gate
        {
            Id = Guid.NewGuid(),
            Name = "gw",
            Community = "public",
            IPAddress = 3232235777,
            TypeOfNetworkDevice = TypeOfNetworkDevice.Extreme
        };

        var dto = mapper.Map<GateDTO>(gate);

        dto.Id.Should().Be(gate.Id);
        dto.Name.Should().Be("gw");
        dto.Community.Should().Be("public");
        dto.IPAddress.Should().Be("192.168.1.1");
        dto.TypeOfNetworkDevice.Should().Be("Extreme");
    }

    [Fact]
    public void CreateGateDtoToGate_ConvertsDottedIpToLongAndParsesEnum()
    {
        var mapper = CreateMapper();
        var dto = new CreateGateDTO
        {
            Name = "gw",
            Community = "public",
            IPAddress = "192.168.1.1",
            TypeOfNetworkDevice = "Juniper"
        };

        var gate = mapper.Map<Gate>(dto);

        gate.Name.Should().Be("gw");
        gate.Community.Should().Be("public");
        gate.IPAddress.Should().Be(3232235777);
        gate.TypeOfNetworkDevice.Should().Be(TypeOfNetworkDevice.Juniper);
    }

    [Fact]
    public void NetworkDeviceToDto_MapsHostAndNestedPorts()
    {
        var mapper = CreateMapper();
        var vlan = new VLAN { Id = Guid.NewGuid(), VLANTag = 100, VLANName = "CLIENTS" };
        var port = new Port
        {
            Id = Guid.NewGuid(),
            InterfaceNumber = 1,
            InterfaceName = "xe-0/0/0",
            InterfaceType = PortType.ethernetCsmacd,
            InterfaceStatus = PortStatus.up,
            InterfaceSpeed = 1_000_000_000,
            MACAddress = "AA:BB:CC:DD:EE:FF",
            Description = "uplink"
        };
        port.VLANs.Add(vlan);
        port.MACTable.Add(new MACEntity { Id = Guid.NewGuid(), MACAddress = "00:1A:2B:3C:4D:5E", PortId = port.Id });
        port.ARPTableOfInterface.Add(new ARPEntity { Id = Guid.NewGuid(), MAC = "00:1A:2B:3C:4D:5E", IPAddress = "192.168.1.10", PortId = port.Id });
        port.ARPTableOfInterface.Add(new ARPEntity { Id = Guid.NewGuid(), MAC = "00:1A:2B:3C:4D:5E", IPAddress = "192.168.1.11", PortId = port.Id });
        port.NetworkTableOfInterface.Add(new TerminatedNetworkEntity
        {
            Id = Guid.NewGuid(),
            PortId = port.Id,
            NetworkAddress = ConvertStringIPAddressToLong.Handle("192.168.1.0"),
            Netmask = ConvertStringIPAddressToLong.Handle("255.255.255.0")
        });
        var device = new NetworkDevice
        {
            Id = Guid.NewGuid(),
            NetworkDeviceName = "gw-1",
            Host = 3232235777,
            GeneralInformation = "OS",
            TypeOfNetworkDevice = TypeOfNetworkDevice.Juniper,
            PortsOfNetworkDevice = [port]
        };

        var dto = mapper.Map<NetworkDeviceDTO>(device);

        dto.Host.Should().Be("192.168.1.1");
        dto.TypeOfNetworkDevice.Should().Be("Juniper");
        dto.PortsOfNetworkDevice.Should().ContainSingle();

        var portDto = dto.PortsOfNetworkDevice[0];
        portDto.InterfaceName.Should().Be("xe-0/0/0");
        portDto.InterfaceType.Should().Be("ethernetCsmacd");
        portDto.InterfaceStatus.Should().Be("up");
        portDto.InterfaceSpeed.Should().Be(1_000_000_000);
        portDto.IsAggregated.Should().BeFalse();
        portDto.VLANs.Should().ContainSingle(v => v.VLANTag == 100 && v.VLANName == "CLIENTS");
        portDto.MacTable.Should().ContainSingle(m => m == "00:1A:2B:3C:4D:5E");

        portDto.ARPTableOfPort.Should().HaveCount(1);
        portDto.ARPTableOfPort["00:1A:2B:3C:4D:5E"].Should().BeEquivalentTo(["192.168.1.10", "192.168.1.11"]);

        portDto.NetworkTableOfPort.Should().HaveCount(1);
        portDto.NetworkTableOfPort["192.168.1.0"].Should().Be("255.255.255.0");
    }

    [Fact]
    public void PortToDto_IsAggregatedTrueWhenChildPortsPresent()
    {
        var mapper = CreateMapper();
        var child = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/0.0" };
        var parent = new Port { Id = Guid.NewGuid(), InterfaceName = "ae0" };
        parent.AggregatedPorts.Add(child);

        var dto = mapper.Map<PortDTO>(parent);

        dto.IsAggregated.Should().BeTrue();
        dto.AggregatedPorts.Should().ContainSingle(p => p.InterfaceName == "xe-0/0/0.0");
    }

    [Fact]
    public void ClientToDetailClientDto_MapsFullGraphIncludingCodTfPlanAndVlans()
    {
        var mapper = CreateMapper();
        var client = new Client
        {
            Id = Guid.NewGuid(),
            IdClient = 5,
            Name = "Alpha",
            NrDogovor = "D-5",
            Nik = "alpha",
            Prim1 = "p1",
            Prim2 = "p2",
            ContactC = "cc",
            TelephoneC = "tc",
            ContactT = "ct",
            TelephoneT = "tt",
            EmailC = "ec",
            EmailT = "et",
            History = "h",
            Working = true,
            AntiDDOS = true,
            Id_COD = 1,
            COD = new COD { Id = Guid.NewGuid(), IdCOD = 1, NameCOD = "DC-1", Region = "R" },
            Id_TfPlan = 2,
            TfPlan = new TfPlan { Id = Guid.NewGuid(), IdTfPlan = 2, NameTfPlan = "Basic", DescTfPlan = "d" },
            SPRVlans = [new SPRVlan { Id = Guid.NewGuid(), IdVlan = 100, IdClient = 5, UseClient = true }]
        };

        var dto = mapper.Map<DetailClientDTO>(client);

        dto.Id.Should().Be(client.Id);
        dto.Name.Should().Be("Alpha");
        dto.Working.Should().BeTrue();
        dto.AntiDDOS.Should().BeTrue();
        dto.Nik.Should().Be("alpha");
        dto.Id_COD.Should().Be(1);
        dto.COD.NameCOD.Should().Be("DC-1");
        dto.Id_TPlan.Should().Be(2);
        dto.TfPlan!.NameTfPlan.Should().Be("Basic");
        dto.SPRVlans.Should().ContainSingle(v => v.IdVlan == 100 && v.UseClient);
    }

    [Fact]
    public void ClientToShortClientDto_MapsBaseFieldsOnly()
    {
        var mapper = CreateMapper();
        var client = new Client { Id = Guid.NewGuid(), IdClient = 7, Name = "Beta", Working = true };

        var dto = mapper.Map<ShortClientDTO>(client);

        dto.Id.Should().Be(client.Id);
        dto.Name.Should().Be("Beta");
        dto.Working.Should().BeTrue();
    }

    [Fact]
    public void TfPlanToDto_MapsAllFields()
    {
        var mapper = CreateMapper();
        var plan = new TfPlan { Id = Guid.NewGuid(), IdTfPlan = 3, NameTfPlan = "Gold", DescTfPlan = "fast" };

        var dto = mapper.Map<TfPlanDTO>(plan);

        dto.Id.Should().Be(plan.Id);
        dto.IdTfPlan.Should().Be(3);
        dto.NameTfPlan.Should().Be("Gold");
        dto.DescTfPlan.Should().Be("fast");
    }

    [Fact]
    public void ArpEntityAndTerminatedNetworkEntityDtos_ConvertLongFieldsToStrings()
    {
        var mapper = CreateMapper();

        var terminated = new TerminatedNetworkEntity
        {
            NetworkAddress = 3232235777,
            Netmask = 4294967040
        };
        var terminatedDto = mapper.Map<TerminatedNetworkEntityDTO>(terminated);
        terminatedDto.NetworkAddress.Should().Be("192.168.1.1");
        terminatedDto.Netmask.Should().Be("255.255.255.0");

        var arp = new ARPEntity { MAC = "00:1A:2B:3C:4D:5E", IPAddress = "10.0.0.1" };
        var arpDto = mapper.Map<ARPEntityDTO>(arp);
        arpDto.MAC.Should().Be("00:1A:2B:3C:4D:5E");
        arpDto.IPAddress.Should().Be("10.0.0.1");
    }
}
