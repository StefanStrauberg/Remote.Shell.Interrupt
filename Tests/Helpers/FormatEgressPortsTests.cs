namespace Tests.Helpers;

public class FormatEgressPortsTests
{
    [Fact]
    public void HandleHuaweiHexString_SingleByte_ReturnsActivePorts()
    {
        // 0xF0 = 11110000 in binary (MSB first), first 4 bits set → indices 0-3
        var result = FormatEgressPorts.HandleHuaweiHexString("F0");
        result.Should().Equal(0, 1, 2, 3);
    }

    [Fact]
    public void HandleHuaweiHexString_MultipleBytes_ReturnsAllActivePorts()
    {
        // 0x01 = 00000001 (MSB first), last bit set → index 7
        // 0x80 = 10000000 (MSB first), first bit set → index 8
        var result = FormatEgressPorts.HandleHuaweiHexString("01 80");
        result.Should().Equal(7, 8);
    }

    [Fact]
    public void HandleHuaweiHexString_AllZeros_ReturnsEmptyArray()
    {
        var result = FormatEgressPorts.HandleHuaweiHexString("00 00");
        result.Should().BeEmpty();
    }

    [Fact]
    public void HandleHuaweiHexString_AllOnes_ReturnsAllPorts()
    {
        var result = FormatEgressPorts.HandleHuaweiHexString("FF");
        result.Should().Equal(0, 1, 2, 3, 4, 5, 6, 7);
    }

    [Fact]
    public void HandleHuaweiHexString_NullInput_ReturnsEmptyArray()
    {
        var result = FormatEgressPorts.HandleHuaweiHexString(null!);
        result.Should().BeEmpty();
    }
}
