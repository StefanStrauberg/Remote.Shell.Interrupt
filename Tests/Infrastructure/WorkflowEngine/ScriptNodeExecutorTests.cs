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

    // Sandbox-escape regression coverage. A Script node's `input`/`context` parameters are
    // wrapped CLR objects (WorkflowContext.Variables), so without an explicit interop lockdown
    // a script could ride Object.GetType() from one of those wrappers into System.Reflection
    // and from there into arbitrary CLR method invocation - the classic Jint embedding escape.
    // These assert the engine actually refuses that path (and its neighbors), not just that the
    // configuration line is present.

    [Fact]
    public async Task ExecuteAsync_ScriptCallsGetTypeOnContext_ReturnsFailedResult_NotTypeInfo()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { var t = context.GetType(); return t.FullName; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ScriptCallsGetTypeOnInput_ReturnsFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return input.GetType().Assembly.FullName; }",
            ["input"] = "raw.interfaces"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_ScriptCallsGetTypeOnConsole_ReturnsFailedResult()
    {
        // console is a plain C# object exposed via SetValue too - the lockdown has to hold for
        // every wrapped CLR object reachable from script, not just input/context.
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { return console.GetType().Name; }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_ScriptAttemptsToImportClrNamespace_ReturnsFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { var File = importNamespace('System.IO').File; return File.Exists('C:\\\\'); }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_InfiniteLoop_TimesOutAndReturnsFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function execute(input, context) { while (true) {} }",
            ["timeoutMs"] = 50
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_UnboundedRecursion_HitsRecursionLimitAndReturnsFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = "function recurse(n) { return recurse(n + 1); } function execute(input, context) { return recurse(0); }"
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_MemoryExhaustionAttempt_HitsMemoryLimitAndReturnsFailedResult()
    {
        var node = ScriptNode(new()
        {
            ["scriptSource"] = """
                function execute(input, context) {
                  var chunks = [];
                  while (true) { chunks.push(new Array(1000000).join('x')); }
                }
                """,
            ["timeoutMs"] = 5000
        });

        var result = await new ScriptNodeExecutor().ExecuteAsync(node, Context(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }
}
