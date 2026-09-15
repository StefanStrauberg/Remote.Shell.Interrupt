using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

public class SetVariableNodeExecutorTests
{
    static WorkflowContext Context() => new("host", "public", new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });

    [Fact]
    public async Task ExecuteAsync_SetsContextVariableAndReturnsItAsOutput()
    {
        var node = new NodeDefinition
        {
            Id = Guid.NewGuid(),
            Type = "SetVariable",
            Config = new() { ["name"] = "vendor", ["value"] = "Huawei" }
        };
        var context = Context();

        var result = await new SetVariableNodeExecutor().ExecuteAsync(node, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Outputs["vendor"].Should().Be("Huawei");
        context.Get<string>("vendor").Should().Be("Huawei");
    }

    [Fact]
    public async Task ExecuteAsync_MissingNameConfig_Throws()
    {
        var node = new NodeDefinition { Id = Guid.NewGuid(), Type = "SetVariable", Config = new() { ["value"] = "x" } };

        var act = async () => await new SetVariableNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
