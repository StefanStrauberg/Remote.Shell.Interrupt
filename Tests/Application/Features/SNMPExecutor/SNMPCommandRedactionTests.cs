namespace Tests.Application.Features.SNMPExecutor;

/// <summary>
/// LoggingBehavior logs every MediatR command via "{Request}", which stringifies the request -
/// these lock in that the SNMP community string (a shared read/write credential on the target
/// device, not just an identifier) never ends up in that log line, the same guarantee
/// LoginCommand/RegisterCommand already give the password.
/// </summary>
public class SNMPCommandRedactionTests
{
    [Fact]
    public void SNMPGetCommand_ToString_NeverIncludesCommunityString()
    {
        var command = new SNMPGetCommand("192.168.1.1", "super-secret-community", "1.3.6.1.2.1.1.1.0");

        command.ToString().Should().NotContain("super-secret-community");
        command.ToString().Should().Contain("192.168.1.1");
        command.ToString().Should().Contain("1.3.6.1.2.1.1.1.0");
    }

    [Fact]
    public void SNMPWalkCommand_ToString_NeverIncludesCommunityString()
    {
        var command = new SNMPWalkCommand("192.168.1.1", "super-secret-community", "1.3.6.1.2.1.1");

        command.ToString().Should().NotContain("super-secret-community");
        command.ToString().Should().Contain("192.168.1.1");
        command.ToString().Should().Contain("1.3.6.1.2.1.1");
    }
}
