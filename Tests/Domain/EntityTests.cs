namespace Tests.Domain;

public class EntityTests
{
    [Fact]
    public void BaseEntity_DefaultValues_HasDefaultGuidIdAndCreatedAt()
    {
        var port = new Port();
        port.Id.Should().Be(Guid.Empty);
        port.CreatedAt.Should().Be(default(DateTime));
        port.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Port_DefaultStringProperties_AreEmptyStrings()
    {
        var port = new Port();
        port.InterfaceName.Should().Be(string.Empty);
        port.MACAddress.Should().Be(string.Empty);
        port.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Port_DefaultCollectionProperties_AreEmptyLists()
    {
        var port = new Port();
        port.ARPTableOfInterface.Should().BeEmpty();
        port.MACTable.Should().BeEmpty();
        port.NetworkTableOfInterface.Should().BeEmpty();
        port.VLANs.Should().BeEmpty();
        port.AggregatedPorts.Should().BeEmpty();
    }

    [Fact]
    public void VLAN_DefaultProperties_AreCorrect()
    {
        var vlan = new VLAN();
        vlan.VLANTag.Should().Be(0);
        vlan.VLANName.Should().Be(string.Empty);
        vlan.Ports.Should().BeEmpty();
    }

    [Fact]
    public void Client_DefaultProperties_AreCorrect()
    {
        var client = new Client();
        client.IdClient.Should().Be(0);
        client.Name.Should().Be(string.Empty);
        client.NrDogovor.Should().Be(string.Empty);
        client.SPRVlans.Should().BeEmpty();
    }

    [Fact]
    public void Gate_InheritsFromBaseEntity()
    {
        var gate = new Gate();
        gate.Id.Should().Be(Guid.Empty); // BaseEntity auto-property defaults to Guid.Empty
    }

    [Fact]
    public void NetworkDevice_DefaultProperties_AreCorrect()
    {
        var device = new NetworkDevice();
        device.Host.Should().Be(0L);
        device.PortsOfNetworkDevice.Should().BeEmpty();
    }

    [Fact]
    public void ARPEntity_DefaultProperties_AreCorrect()
    {
        var arp = new ARPEntity();
        arp.MAC.Should().Be(string.Empty);
        arp.IPAddress.Should().Be(string.Empty);
    }

    [Fact]
    public void MACEntity_DefaultProperties_AreCorrect()
    {
        var mac = new MACEntity();
        mac.MACAddress.Should().Be(string.Empty);
    }

    [Fact]
    public void TfPlan_DescTfPlan_IsNonNullableString()
    {
        var tfPlan = new TfPlan();
        tfPlan.DescTfPlan.Should().Be(string.Empty);
    }

    [Fact]
    public void COD_DefaultProperties_AreCorrect()
    {
        var cod = new COD();
        cod.IdCOD.Should().Be(0);
        cod.NameCOD.Should().Be(string.Empty);
    }

    [Fact]
    public void SPRVlan_DefaultProperties_AreCorrect()
    {
        var sprVlan = new SPRVlan();
        sprVlan.IdVlan.Should().Be(0);
    }

    [Fact]
    public void TypeOfNetworkDevice_HasExpectedValues()
    {
        Enum.GetNames<TypeOfNetworkDevice>().Should().Contain("Huawei");
        Enum.GetNames<TypeOfNetworkDevice>().Should().Contain("Juniper");
        Enum.GetNames<TypeOfNetworkDevice>().Should().Contain("Extreme");
        Enum.GetNames<TypeOfNetworkDevice>().Should().Contain("Cisco");
        Enum.GetNames<TypeOfNetworkDevice>().Should().Contain("FortiGate");
    }

    [Fact]
    public void PortType_HasExpectedValues()
    {
        Enum.GetNames<PortType>().Should().Contain("other");
        Enum.GetNames<PortType>().Should().Contain("ethernetCsmacd");
    }

    [Fact]
    public void PortStatus_HasExpectedValues()
    {
        Enum.GetNames<PortStatus>().Should().Contain("up");
        Enum.GetNames<PortStatus>().Should().Contain("down");
    }
}
