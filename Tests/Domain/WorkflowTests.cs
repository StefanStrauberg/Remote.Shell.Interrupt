using Remote.Shell.Interrupt.Storehouse.Domain.Gateway;
using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;

namespace Tests.Domain;

public class WorkflowContextTests
{
    static WorkflowContext CreateContext()
        => new(new NetworkDevice { Id = Guid.NewGuid(), NetworkDeviceName = "gw" },
               new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });

    [Fact]
    public void SetAndGet_MatchingRuntimeType_RoundTripsValue()
    {
        var context = CreateContext();

        context.Set("count", 5);

        context.Get<int>("count").Should().Be(5);
    }

    [Fact]
    public void Get_MissingKey_ReturnsDefault()
    {
        var context = CreateContext();

        context.Get<int>("missing").Should().Be(0);
        context.Get<string>("missing").Should().BeNull();
    }

    [Fact]
    public void Get_StoredStringConvertedToInt_UsesConvertChangeType()
    {
        var context = CreateContext();
        context.Set("count", "42");

        context.Get<int>("count").Should().Be(42);
    }

    [Fact]
    public void Get_StoredStringConvertedToGuid_ParsesGuid()
    {
        var context = CreateContext();
        var id = Guid.NewGuid();
        context.Set("deviceId", id.ToString());

        context.Get<Guid>("deviceId").Should().Be(id);
    }

    [Fact]
    public void Get_StoredStringConvertedToEnum_ParsesEnumCaseInsensitively()
    {
        var context = CreateContext();
        context.Set("vendor", "cisco");

        context.Get<TypeOfNetworkDevice>("vendor").Should().Be(TypeOfNetworkDevice.Cisco);
    }

    [Fact]
    public void Get_NullableIntTarget_ConvertsAgainstUnderlyingType()
    {
        var context = CreateContext();
        context.Set("count", "7");

        context.Get<int?>("count").Should().Be(7);
    }

    [Fact]
    public void Get_NullableGuidTarget_ParsesUnderlyingGuid()
    {
        var context = CreateContext();
        var id = Guid.NewGuid();
        context.Set("deviceId", id.ToString());

        context.Get<Guid?>("deviceId").Should().Be(id);
    }

    [Fact]
    public void Contains_ReflectsWhetherKeyWasSet()
    {
        var context = CreateContext();

        context.Contains("missing").Should().BeFalse();
        context.Set("present", 1);
        context.Contains("present").Should().BeTrue();
    }

    [Fact]
    public void Variables_ExposesAllSetEntries()
    {
        var context = CreateContext();
        context.Set("a", 1);
        context.Set("b", "two");

        context.Variables.Should().ContainKeys("a", "b");
        context.Variables["a"].Should().Be(1);
        context.Variables["b"].Should().Be("two");
    }

    [Fact]
    public void CurrentNodeId_IsPubliclySettable()
    {
        var context = CreateContext();
        var nodeId = Guid.NewGuid();

        context.CurrentNodeId = nodeId;

        context.CurrentNodeId.Should().Be(nodeId);
    }

    [Fact]
    public void Device_And_Workflow_AreExposedFromConstructor()
    {
        var device = new NetworkDevice { Id = Guid.NewGuid(), NetworkDeviceName = "gw-1" };
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf-1" };

        var context = new WorkflowContext(device, workflow);

        context.Device.Should().BeSameAs(device);
        context.Workflow.Should().BeSameAs(workflow);
    }
}

public class NodeResultTests
{
    [Fact]
    public void Ok_ReturnsSuccessfulResultWithNoDecisionOrError()
    {
        var result = NodeResult.Ok();

        result.Success.Should().BeTrue();
        result.Decision.Should().BeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void WithDecision_ReturnsSuccessfulResultCarryingDecision()
    {
        var result = NodeResult.WithDecision("Huawei");

        result.Success.Should().BeTrue();
        result.Decision.Should().Be("Huawei");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_ReturnsUnsuccessfulResultCarryingError()
    {
        var result = NodeResult.Failed("timeout");

        result.Success.Should().BeFalse();
        result.Error.Should().Be("timeout");
        result.Decision.Should().BeNull();
    }
}

public class WorkflowDefinitionGraphTests
{
    [Fact]
    public void WorkflowDefinition_HoldsNodesAndEdges()
    {
        var startNode = new NodeDefinition { Id = Guid.NewGuid(), Type = "Start", Name = "Start" };
        var endNode = new NodeDefinition { Id = Guid.NewGuid(), Type = "End", Name = "End" };
        var edge = new EdgeDefinition { FromNodeId = startNode.Id, ToNodeId = endNode.Id, Condition = null, Priority = 0 };

        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "wf",
            Version = 1,
            StartNodeId = startNode.Id,
            Nodes = [startNode, endNode],
            Edges = [edge]
        };

        workflow.Nodes.Should().HaveCount(2);
        workflow.Edges.Should().ContainSingle();
        workflow.StartNodeId.Should().Be(startNode.Id);
        workflow.Edges[0].FromNodeId.Should().Be(startNode.Id);
        workflow.Edges[0].ToNodeId.Should().Be(endNode.Id);
    }
}
