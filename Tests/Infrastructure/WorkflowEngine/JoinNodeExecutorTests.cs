using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

public class JoinNodeExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsSuccessfulEmptyResult()
    {
        var node = new NodeDefinition { Id = Guid.NewGuid(), Type = "Join" };
        var context = new WorkflowContext("host", "public", new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });

        var result = await new JoinNodeExecutor().ExecuteAsync(node, context, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Outputs.Should().BeEmpty();
        result.Decision.Should().BeNull();
    }
}
