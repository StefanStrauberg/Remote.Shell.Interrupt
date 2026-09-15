using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.SNMPRep;
using Remote.Shell.Interrupt.Storehouse.Domain.SNMP;
using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

public class SnmpGetNodeExecutorTests
{
    readonly ISNMPCommandExecutor _snmp = Substitute.For<ISNMPCommandExecutor>();

    [Fact]
    public async Task ExecuteAsync_UsesContextHostAndCommunity_WritesResponseDataToOutput()
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" };
        var context = new WorkflowContext("10.0.0.5", "public", workflow);
        _snmp.GetCommand("10.0.0.5", "public", "1.3.6.1.2.1.1.1.0", Arg.Any<CancellationToken>(), false)
             .Returns(new SNMPResponse { OID = "1.3.6.1.2.1.1.1.0", Data = "Cisco IOS" });
        var node = new NodeDefinition
        {
            Id = Guid.NewGuid(),
            Type = "SnmpGet",
            Config = new() { ["oid"] = "1.3.6.1.2.1.1.1.0", ["output"] = "device.sysDescr" }
        };

        var result = await new SnmpGetNodeExecutor(_snmp).ExecuteAsync(node, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Outputs["device.sysDescr"].Should().Be("Cisco IOS");
    }

    [Fact]
    public async Task ExecuteAsync_MissingOidConfig_Throws()
    {
        var node = new NodeDefinition { Id = Guid.NewGuid(), Type = "SnmpGet", Config = new() { ["output"] = "x" } };
        var context = new WorkflowContext("host", "public", new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });

        var act = async () => await new SnmpGetNodeExecutor(_snmp).ExecuteAsync(node, context, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

public class SnmpWalkNodeExecutorTests
{
    readonly ISNMPCommandExecutor _snmp = Substitute.For<ISNMPCommandExecutor>();

    [Fact]
    public async Task ExecuteAsync_WritesAllResponseValuesAsArray()
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" };
        var context = new WorkflowContext("10.0.0.5", "public", workflow);
        _snmp.WalkCommand("10.0.0.5", "public", "1.3.6.1.2.1.2.2.1.2", Arg.Any<CancellationToken>(), false, 20)
             .Returns(
             [
                 new SNMPResponse { OID = "1.3.6.1.2.1.2.2.1.2.1", Data = "GE0/0/1" },
                 new SNMPResponse { OID = "1.3.6.1.2.1.2.2.1.2.2", Data = "GE0/0/2" }
             ]);
        var node = new NodeDefinition
        {
            Id = Guid.NewGuid(),
            Type = "SnmpWalk",
            Config = new() { ["oid"] = "1.3.6.1.2.1.2.2.1.2", ["output"] = "raw.interfaces" }
        };

        var result = await new SnmpWalkNodeExecutor(_snmp).ExecuteAsync(node, context, CancellationToken.None);

        var entries = result.Outputs["raw.interfaces"].Should().BeAssignableTo<List<Dictionary<string, object?>>>().Subject;
        entries.Should().HaveCount(2);
        entries[0]["oid"].Should().Be("1.3.6.1.2.1.2.2.1.2.1");
        entries[0]["data"].Should().Be("GE0/0/1");
        entries[1]["oid"].Should().Be("1.3.6.1.2.1.2.2.1.2.2");
        entries[1]["data"].Should().Be("GE0/0/2");
    }
}
