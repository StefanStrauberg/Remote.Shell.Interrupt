using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.CreateWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.UpdateWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

namespace Tests.Validations;

public class CreateWorkflowCommandValidatorTests
{
    readonly CreateWorkflowCommandValidator _validator = new();

    static (Guid Start, Guid End) Ids() => (Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public void ValidGraph_PassesValidation()
    {
        var (start, end) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            Version = 1,
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start", Name = "Start" }, new() { Id = end, Type = "End", Name = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var (start, end) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void NoNodes_FailsValidation()
    {
        var dto = new CreateWorkflowDTO { Name = "wf" };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void UnknownNodeType_FailsValidation()
    {
        var (start, end) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "Bogus" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void NoEndNode_FailsValidation()
    {
        var (start, _) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void StartNodeIdNotAmongNodes_FailsValidation()
    {
        var (start, end) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = Guid.NewGuid(),
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void EdgeReferencesUnknownNode_FailsValidation()
    {
        var (start, end) = Ids();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = Guid.NewGuid() }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void TwoStartNodes_FailsValidation()
    {
        var (start, end) = Ids();
        var secondStart = Guid.NewGuid();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = secondStart, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void JoinAndSetVariableNodes_PassValidation()
    {
        var (start, end) = Ids();
        var join = Guid.NewGuid();
        var setVar = Guid.NewGuid();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes =
            [
                new() { Id = start, Type = "Start" },
                new() { Id = setVar, Type = "SetVariable", Config = new() { ["name"] = "vendor", ["value"] = "Huawei" } },
                new() { Id = join, Type = "Join" },
                new() { Id = end, Type = "End" }
            ],
            Edges =
            [
                new() { FromNodeId = start, ToNodeId = setVar },
                new() { FromNodeId = setVar, ToNodeId = join },
                new() { FromNodeId = join, ToNodeId = end }
            ]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ScriptNodeMissingScriptSource_FailsValidation()
    {
        var (start, end) = Ids();
        var script = Guid.NewGuid();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = script, Type = "Script" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = script }, new() { FromNodeId = script, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(40000)]
    public void ScriptNodeTimeoutOutOfRange_FailsValidation(int timeoutMs)
    {
        var (start, end) = Ids();
        var script = Guid.NewGuid();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes =
            [
                new() { Id = start, Type = "Start" },
                new() { Id = script, Type = "Script", Config = new() { ["scriptSource"] = "function execute(i,c){return i;}", ["timeoutMs"] = timeoutMs } },
                new() { Id = end, Type = "End" }
            ],
            Edges = [new() { FromNodeId = start, ToNodeId = script }, new() { FromNodeId = script, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ScriptNodeValidSourceAndTimeout_PassesValidation()
    {
        var (start, end) = Ids();
        var script = Guid.NewGuid();
        var dto = new CreateWorkflowDTO
        {
            Name = "wf",
            StartNodeId = start,
            Nodes =
            [
                new() { Id = start, Type = "Start" },
                new() { Id = script, Type = "Script", Config = new() { ["scriptSource"] = "function execute(i,c){return i;}", ["timeoutMs"] = 1500 } },
                new() { Id = end, Type = "End" }
            ],
            Edges = [new() { FromNodeId = start, ToNodeId = script }, new() { FromNodeId = script, ToNodeId = end }]
        };

        _validator.Validate(new CreateWorkflowCommand(dto)).IsValid.Should().BeTrue();
    }
}

public class UpdateWorkflowCommandValidatorTests
{
    readonly UpdateWorkflowCommandValidator _validator = new();

    [Fact]
    public void EmptyId_FailsValidation()
    {
        var start = Guid.NewGuid();
        var end = Guid.NewGuid();
        var dto = new UpdateWorkflowDTO
        {
            Id = Guid.Empty,
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new UpdateWorkflowCommand(dto)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidGraph_PassesValidation()
    {
        var start = Guid.NewGuid();
        var end = Guid.NewGuid();
        var dto = new UpdateWorkflowDTO
        {
            Id = Guid.NewGuid(),
            Name = "wf",
            StartNodeId = start,
            Nodes = [new() { Id = start, Type = "Start" }, new() { Id = end, Type = "End" }],
            Edges = [new() { FromNodeId = start, ToNodeId = end }]
        };

        _validator.Validate(new UpdateWorkflowCommand(dto)).IsValid.Should().BeTrue();
    }
}
