using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

public class DecisionNodeExecutorTests
{
    static WorkflowContext Context() => new("host", "public", new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });

    [Fact]
    public async Task ExecuteAsync_ValueMode_ReturnsRawVariableAsDecision()
    {
        var context = Context();
        context.Set("device.vendor", "Huawei");
        var node = new NodeDefinition { Id = Guid.NewGuid(), Type = "Decision", Config = new() { ["variable"] = "device.vendor" } };

        var result = await new DecisionNodeExecutor().ExecuteAsync(node, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Decision.Should().Be("Huawei");
    }

    [Theory]
    [InlineData("12.1", "MODERN")]
    [InlineData("10.0", "MODERN")]
    [InlineData("9.9", "LEGACY")]
    [InlineData("8.5", "LEGACY")]
    [InlineData("not-a-version", "LEGACY")]
    public async Task ExecuteAsync_FirmwareMajorMode_BucketsByMajorVersion(string firmware, string expectedDecision)
    {
        var context = Context();
        context.Set("device.firmware", firmware);
        var node = new NodeDefinition
        {
            Id = Guid.NewGuid(),
            Type = "Decision",
            Config = new() { ["variable"] = "device.firmware", ["mode"] = "firmware-major" }
        };

        var result = await new DecisionNodeExecutor().ExecuteAsync(node, context, CancellationToken.None);

        result.Decision.Should().Be(expectedDecision);
    }

    [Fact]
    public async Task ExecuteAsync_MissingVariableConfig_Throws()
    {
        var node = new NodeDefinition { Id = Guid.NewGuid(), Type = "Decision" };

        var act = async () => await new DecisionNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_UnknownMode_Throws()
    {
        var node = new NodeDefinition
        {
            Id = Guid.NewGuid(),
            Type = "Decision",
            Config = new() { ["variable"] = "x", ["mode"] = "bogus" }
        };

        var act = async () => await new DecisionNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
