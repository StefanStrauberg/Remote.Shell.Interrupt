using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Tests.Infrastructure.WorkflowEngine;

public class ScriptNodeExecutorTests
{
    static WorkflowContext Context()
    {
        var context = new WorkflowContext("host", "public", new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf" });
        context.Set("raw.interfaces", new[] { "GE0/0/1", "GE0/0/2" });
        context.Set("vendor", "Huawei");
        return context;
    }

    static NodeDefinition ScriptNode(Dictionary<string, object?> config)
        => new() { Id = Guid.NewGuid(), Key = "script-1", Type = "Script", Config = config };

    [Fact]
    public async Task ExecuteAsync_PlainReturnValue_WrittenToOutputPath()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return input.map((x, i) => ({ index: i + 1, name: x })); }",
            ["input"] = "raw.interfaces",
            ["output"] = "normalized.ports"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Outputs.Should().ContainKey("normalized.ports");
    }

    [Fact]
    public async Task ExecuteAsync_NoOutputConfigured_DefaultsToScriptDotKey()
    {
        var node = ScriptNode(new() { ["scriptSource"] = "function execute(input, context) { return 42; }" });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Outputs.Should().ContainKey("script.script-1");
        result.Outputs["script.script-1"].Should().Be(42);
    }

    [Fact]
    public async Task ExecuteAsync_NodeResultShapedReturn_UsesSuccessDecisionOutputsError()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = """
                function execute(input, context) {
                  return {
                    success: true,
                    decision: 'HAS_DATA',
                    outputs: { 'result.count': input.length }
                  };
                }
                """,
            ["input"] = "raw.interfaces"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Decision.Should().Be("HAS_DATA");
        result.Outputs["result.count"].Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_NodeResultShapedReturn_SuccessFalse_MapsToFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return { success: false, error: 'bad input' }; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("bad input");
    }

    [Fact]
    public async Task ExecuteAsync_EmptyInputPath_PassesWholeContextAsInput()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return input['vendor']; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Outputs.Values.Should().Contain("Huawei");
    }

    [Fact]
    public async Task ExecuteAsync_ScriptReadsContextParamByBracketNotation()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return context['raw.interfaces'].length; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Outputs.Values.Should().Contain(2d);
    }

    [Fact]
    public async Task ExecuteAsync_ConsoleLog_CapturedInLogs()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { console.log('hello', 42); return 1; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Logs.Should().ContainSingle(l => l.Contains("hello") && l.Contains("42"));
    }

    [Fact]
    public async Task ExecuteAsync_ScriptThrows_ReturnsFailedResultWithLogsPreserved()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { console.log('before throw'); throw new Error('boom'); }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("boom");
        result.Logs.Should().ContainSingle(l => l.Contains("before throw"));
    }

    [Fact]
    public async Task ExecuteAsync_MissingScriptSourceConfig_Throws()
    {
        var node = ScriptNode([]);

        var act = async () => await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_MissingExecuteFunction_ReturnsFailedResult()
    {
        var node = ScriptNode(new() { ["scriptSource"] = "const x = 1;" });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }
}
