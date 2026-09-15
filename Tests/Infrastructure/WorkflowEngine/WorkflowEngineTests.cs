using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

/// <summary>
/// Ports the four scenarios from WorkflowExample/EXPECTED_FLOW.md (Huawei modern/legacy,
/// Juniper, unknown-vendor -> DEFAULT) against the ported engine, using only the built-in
/// Start/Decision/End executors - no SNMP/Script involved, so these exercise pure
/// edge-resolution and decision-mode logic.
/// </summary>
public class WorkflowEngineTests
{
    static (WorkflowDefinition Workflow, Guid Start, Guid VendorDecision, Guid FirmwareDecision,
            Guid HuaweiModernEnd, Guid HuaweiLegacyEnd, Guid JuniperEnd, Guid DefaultEnd) BuildGraph()
    {
        var start = Node("Start", "Start");
        var vendorDecision = Node("Decision", "Choose vendor", new() { ["variable"] = "device.vendor" });
        var firmwareDecision = Node("Decision", "Choose firmware", new()
        {
            ["variable"] = "device.firmware",
            ["mode"] = "firmware-major"
        });
        var huaweiModernEnd = Node("End", "Huawei Modern End");
        var huaweiLegacyEnd = Node("End", "Huawei Legacy End");
        var juniperEnd = Node("End", "Juniper End");
        var defaultEnd = Node("End", "Default End");

        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "test-wf",
            Version = 1,
            StartNodeId = start.Id,
            Nodes = [start, vendorDecision, firmwareDecision, huaweiModernEnd, huaweiLegacyEnd, juniperEnd, defaultEnd],
            Edges =
            [
                Edge(start, vendorDecision),
                Edge(vendorDecision, firmwareDecision, "Huawei", 1),
                Edge(vendorDecision, juniperEnd, "Juniper", 2),
                Edge(vendorDecision, defaultEnd, "DEFAULT", 100),
                Edge(firmwareDecision, huaweiModernEnd, "MODERN", 1),
                Edge(firmwareDecision, huaweiLegacyEnd, "LEGACY", 2)
            ]
        };

        return (workflow, start.Id, vendorDecision.Id, firmwareDecision.Id, huaweiModernEnd.Id, huaweiLegacyEnd.Id, juniperEnd.Id, defaultEnd.Id);
    }

    static NodeDefinition Node(string type, string name, Dictionary<string, object?>? config = null)
        => new() { Id = Guid.NewGuid(), Type = type, Name = name, Config = config ?? [] };

    static EdgeDefinition Edge(NodeDefinition from, NodeDefinition to, string? condition = null, int priority = 0)
        => new() { FromNodeId = from.Id, ToNodeId = to.Id, Condition = condition, Priority = priority };

    static Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.WorkflowEngine BuildEngine()
    {
        var resolver = new WorkflowNodeResolver([new StartNodeExecutor(), new EndNodeExecutor(), new DecisionNodeExecutor()]);
        return new(resolver);
    }

    [Fact]
    public async Task Execute_HuaweiModernFirmware_ReachesHuaweiModernEnd()
    {
        var (workflow, _, _, _, _, _, _, _) = BuildGraph();
        var context = new WorkflowContext("host", "public", workflow);
        context.Set("device.vendor", "Huawei");
        context.Set("device.firmware", "12.1");

        var result = await BuildEngine().ExecuteAsync(workflow, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Steps.Last().NodeName.Should().Be("Huawei Modern End");
    }

    [Fact]
    public async Task Execute_HuaweiLegacyFirmware_ReachesHuaweiLegacyEnd()
    {
        var (workflow, _, _, _, _, _, _, _) = BuildGraph();
        var context = new WorkflowContext("host", "public", workflow);
        context.Set("device.vendor", "Huawei");
        context.Set("device.firmware", "8.5");

        var result = await BuildEngine().ExecuteAsync(workflow, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Steps.Last().NodeName.Should().Be("Huawei Legacy End");
    }

    [Fact]
    public async Task Execute_Juniper_ReachesJuniperEndWithoutTouchingFirmwareDecision()
    {
        var (workflow, _, _, _, _, _, _, _) = BuildGraph();
        var context = new WorkflowContext("host", "public", workflow);
        context.Set("device.vendor", "Juniper");

        var result = await BuildEngine().ExecuteAsync(workflow, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Steps.Last().NodeName.Should().Be("Juniper End");
        result.Steps.Should().NotContain(s => s.NodeName == "Choose firmware");
    }

    [Fact]
    public async Task Execute_UnknownVendor_FallsBackToDefaultEdge()
    {
        var (workflow, _, _, _, _, _, _, _) = BuildGraph();
        var context = new WorkflowContext("host", "public", workflow);
        context.Set("device.vendor", "Cisco");

        var result = await BuildEngine().ExecuteAsync(workflow, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Steps.Last().NodeName.Should().Be("Default End");
    }

    [Fact]
    public async Task Execute_NodeWithNoOutgoingEdges_ReturnsFailureWithPartialTrace()
    {
        var deadEnd = Node("Decision", "Dead end", new() { ["variable"] = "x" });
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "broken",
            StartNodeId = deadEnd.Id,
            Nodes = [deadEnd],
            Edges = []
        };
        var context = new WorkflowContext("host", "public", workflow);

        var result = await BuildEngine().ExecuteAsync(workflow, context, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        result.Steps.Should().ContainSingle();
    }
}
