using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IWorkflowRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Workflow;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.CreateWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.DeleteWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.ExecuteWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.PublishWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.ArchiveWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.UpdateWorkflow;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Queries.GetWorkflowById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Queries.GetWorkflowsByFilter;
using Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;
using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

namespace Tests.Application.Features.Workflows;

public abstract class WorkflowHandlerTestBase
{
    protected readonly IWorkflowDefinitionRepository Workflows = Substitute.For<IWorkflowDefinitionRepository>();
    protected readonly IWorkflowUnitOfWork UnitOfWork = Substitute.For<IWorkflowUnitOfWork>();
    protected readonly IWorkflowSpecification Specification = Substitute.For<IWorkflowSpecification>();
    protected readonly IQueryFilterParser Parser = new CommonQueryFilterParser();
    protected readonly IMapper Mapper = Substitute.For<IMapper>();

    protected WorkflowHandlerTestBase()
    {
        UnitOfWork.Workflows.Returns(Workflows);
        Specification.Clone().Returns(Specification);
    }

    protected static WorkflowNodeDTO Node(Guid id, string type, string name = "n")
        => new() { Id = id, Type = type, Name = name };

    protected static WorkflowEdgeDTO Edge(Guid from, Guid to, string? condition = null, int priority = 0)
        => new() { FromNodeId = from, ToNodeId = to, Condition = condition, Priority = priority };
}

public class CreateWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly Guid _startId = Guid.NewGuid();
    readonly Guid _endId = Guid.NewGuid();
    CreateWorkflowCommand Command()
    {
        var dto = new CreateWorkflowDTO
        {
            Name = "wf-1",
            Version = 1,
            StartNodeId = _startId,
            Nodes = [Node(_startId, "Start", "Start"), Node(_endId, "End", "End")],
            Edges = [Edge(_startId, _endId)]
        };
        return new CreateWorkflowCommand(dto);
    }

    readonly CreateWorkflowCommandHandler _handler;

    public CreateWorkflowCommandHandlerTests()
    {
        Mapper.Map<WorkflowDefinition>(Arg.Any<object>()).Returns(ci =>
        {
            var dto = ci.ArgAt<CreateWorkflowDTO>(0);
            return new WorkflowDefinition { Name = dto.Name, Version = dto.Version, StartNodeId = dto.StartNodeId };
        });
        _handler = new CreateWorkflowCommandHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    [Fact]
    public async Task Handle_DuplicateName_ThrowsEntityAlreadyExists()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await _handler.Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<EntityAlreadyExists>();
        Workflows.DidNotReceiveWithAnyArgs().InsertOne(default!);
    }

    [Fact]
    public async Task Handle_NoDuplicate_InsertsInsideTransactionAndCompletes()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);
        WorkflowDefinition? inserted = null;
        Workflows.InsertOne(Arg.Do<WorkflowDefinition>(w => inserted = w));

        await _handler.Handle(Command(), CancellationToken.None);

        inserted.Should().NotBeNull();
        inserted!.Name.Should().Be("wf-1");
        UnitOfWork.Received().StartTransaction();
        UnitOfWork.Received().Complete();
    }

    [Fact]
    public async Task Handle_NoDuplicate_ReturnsInsertedEntitysId()
    {
        // Regression: the caller used to have no way to learn the new workflow's ID from this
        // command's response and had to re-query by name, which is ambiguous under a race. The
        // ID here comes from the DB's gen_random_uuid() default via SaveChanges() - simulate
        // that by having InsertOne assign an ID the way EF Core would after Complete().
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);
        var generatedId = Guid.NewGuid();
        Workflows.InsertOne(Arg.Do<WorkflowDefinition>(w => w.Id = generatedId));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Should().Be(generatedId);
    }

    [Fact]
    public async Task Handle_DuplicateCheckUsesParsedFilterOnName()
    {
        var realSpec = new WorkflowSpecification();
        var handler = new CreateWorkflowCommandHandler(UnitOfWork, realSpec, Parser, Mapper);
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        await handler.Handle(Command(), CancellationToken.None);

        await Workflows.Received().AnyByQueryAsync(
            Arg.Is<ISpecification<WorkflowDefinition>>(s => s.Criterias != null), Arg.Any<CancellationToken>());
    }
}

public class DeleteWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly DeleteWorkflowCommandHandler _handler;
    readonly WorkflowDefinition _workflow = new() { Id = Guid.NewGuid(), Name = "wf-1" };

    public DeleteWorkflowCommandHandlerTests()
        => _handler = new DeleteWorkflowCommandHandler(UnitOfWork, Specification, Parser);

    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await ((IRequestHandler<DeleteWorkflowCommand, Unit>)_handler)
            .Handle(new DeleteWorkflowCommand(_workflow.Id), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        Workflows.DidNotReceiveWithAnyArgs().DeleteOne(default!);
        UnitOfWork.DidNotReceive().Complete();
    }

    [Fact]
    public async Task Handle_WorkflowExists_DeletesAndCompletes()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneShortAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(_workflow);

        await ((IRequestHandler<DeleteWorkflowCommand, Unit>)_handler)
            .Handle(new DeleteWorkflowCommand(_workflow.Id), CancellationToken.None);

        Workflows.Received().DeleteOne(_workflow);
        UnitOfWork.Received().Complete();
    }
}

public class UpdateWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly UpdateWorkflowCommandHandler _handler;
    readonly Guid _startId = Guid.NewGuid();
    readonly Guid _endId = Guid.NewGuid();

    public UpdateWorkflowCommandHandlerTests()
    {
        Mapper.Map<List<NodeDefinition>>(Arg.Any<object>()).Returns(ci =>
            ci.ArgAt<List<WorkflowNodeDTO>>(0).Select(n => new NodeDefinition { Id = n.Id, Type = n.Type, Name = n.Name }).ToList());
        Mapper.Map<List<EdgeDefinition>>(Arg.Any<object>()).Returns(ci =>
            ci.ArgAt<List<WorkflowEdgeDTO>>(0).Select(e => new EdgeDefinition { FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Condition = e.Condition, Priority = e.Priority }).ToList());

        _handler = new UpdateWorkflowCommandHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    UpdateWorkflowDTO Dto(Guid id) => new()
    {
        Id = id,
        Name = "wf-updated",
        Version = 2,
        StartNodeId = _startId,
        Nodes = [Node(_startId, "Start"), Node(_endId, "End")],
        Edges = [Edge(_startId, _endId)]
    };

    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _handler.Handle(new UpdateWorkflowCommand(Dto(Guid.NewGuid())), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        UnitOfWork.DidNotReceive().Complete();
    }

    [Fact]
    public async Task Handle_WorkflowExists_ReplacesGraphAndCompletes()
    {
        var existing = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "old", Version = 1 };
        existing.Nodes.Add(new NodeDefinition { Id = Guid.NewGuid(), Type = "Start", Name = "stale" });

        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneWithChildrenTrackedAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(existing);

        await _handler.Handle(new UpdateWorkflowCommand(Dto(existing.Id)), CancellationToken.None);

        existing.Name.Should().Be("wf-updated");
        existing.Version.Should().Be(2);
        existing.StartNodeId.Should().Be(_startId);
        existing.Nodes.Should().HaveCount(2);
        existing.Nodes.Should().OnlyContain(n => n.Id == _startId || n.Id == _endId);
        existing.Edges.Should().ContainSingle(e => e.FromNodeId == _startId && e.ToNodeId == _endId);
        UnitOfWork.Received().Complete();
    }

    [Fact]
    public async Task Handle_WorkflowIsPublished_ThrowsWorkflowNotEditable()
    {
        var existing = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "old", Status = WorkflowStatus.Published };

        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneWithChildrenTrackedAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(existing);

        var act = async () => await _handler.Handle(new UpdateWorkflowCommand(Dto(existing.Id)), CancellationToken.None);

        await act.Should().ThrowAsync<WorkflowNotEditableException>();
        UnitOfWork.DidNotReceive().Complete();
    }
}

public class GetWorkflowByIdQueryHandlerTests : WorkflowHandlerTestBase
{
    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        var handler = new GetWorkflowByIdQueryHandler(UnitOfWork, Specification, Parser, Mapper);
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await ((IRequestHandler<GetWorkflowByIdQuery, WorkflowDTO>)handler)
            .Handle(new GetWorkflowByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_WorkflowExists_ReturnsMappedGraph()
    {
        var mapperConfig = new TypeAdapterConfig();
        mapperConfig.Scan(typeof(WorkflowDTO).Assembly);
        var realMapper = new Mapper(mapperConfig);
        var handler = new GetWorkflowByIdQueryHandler(UnitOfWork, Specification, Parser, realMapper);

        var startId = Guid.NewGuid();
        var endId = Guid.NewGuid();
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf", Version = 1, StartNodeId = startId };
        workflow.Nodes.Add(new NodeDefinition { Id = startId, Type = "Start", Name = "Start" });
        workflow.Nodes.Add(new NodeDefinition { Id = endId, Type = "End", Name = "End" });
        workflow.Edges.Add(new EdgeDefinition { FromNodeId = startId, ToNodeId = endId });

        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneWithChildrenAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(workflow);

        var result = await ((IRequestHandler<GetWorkflowByIdQuery, WorkflowDTO>)handler)
            .Handle(new GetWorkflowByIdQuery(workflow.Id), CancellationToken.None);

        result.Id.Should().Be(workflow.Id);
        result.Name.Should().Be("wf");
        result.Nodes.Should().HaveCount(2);
        result.Edges.Should().ContainSingle(e => e.FromNodeId == startId && e.ToNodeId == endId);
    }
}

public class GetWorkflowsByFilterQueryHandlerTests : WorkflowHandlerTestBase
{
    readonly GetWorkflowsByFilterQueryHandler _handler;
    readonly List<WorkflowDefinition> _workflows =
    [
        new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf-1" },
        new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf-2" }
    ];

    public GetWorkflowsByFilterQueryHandlerTests()
    {
        Mapper.Map<IEnumerable<WorkflowSummaryDTO>>(Arg.Any<object>()).Returns(
            [new WorkflowSummaryDTO { Name = "wf-1" }, new WorkflowSummaryDTO { Name = "wf-2" }]);
        _handler = new GetWorkflowsByFilterQueryHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    [Fact]
    public async Task Handle_Paginated_CountsAndReturnsPagedList()
    {
        Workflows.GetManyWithChildrenAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>())
                 .Returns(_workflows);
        Workflows.GetCountAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(25);
        var query = new GetWorkflowsByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(25);
        result.Should().HaveCount(2);
        await Workflows.Received().GetCountAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedList()
    {
        Workflows.GetManyWithChildrenAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>())
                 .Returns([]);

        var result = await _handler.Handle(
            new GetWorkflowsByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 }),
            CancellationToken.None);

        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}

public class ExecuteWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly IWorkflowEngine Engine = Substitute.For<IWorkflowEngine>();
    readonly WorkflowDefinition _workflow = new() { Id = Guid.NewGuid(), Name = "wf" };

    ExecuteWorkflowCommandHandler Handler() => new(UnitOfWork, Specification, Parser, Engine, Mapper);

    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await Handler().Handle(
            new ExecuteWorkflowCommand(Guid.NewGuid(), "10.0.0.1", "public"), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        await Engine.DidNotReceiveWithAnyArgs().ExecuteAsync(default!, default!, default);
    }

    [Fact]
    public async Task Handle_WorkflowFound_RunsEngineWithHostCommunityContextAndMapsResult()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneWithChildrenAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(_workflow);

        WorkflowContext? capturedContext = null;
        Engine.ExecuteAsync(Arg.Any<WorkflowDefinition>(), Arg.Do<WorkflowContext>(c => capturedContext = c), Arg.Any<CancellationToken>())
              .Returns(new WorkflowExecutionResult { Success = true });
        Mapper.Map<WorkflowExecutionResultDTO>(Arg.Any<object>()).Returns(new WorkflowExecutionResultDTO { Success = true });

        var result = await Handler().Handle(
            new ExecuteWorkflowCommand(_workflow.Id, "10.0.0.1", "public"), CancellationToken.None);

        result.Success.Should().BeTrue();
        capturedContext.Should().NotBeNull();
        capturedContext!.Host.Should().Be("10.0.0.1");
        capturedContext.Community.Should().Be("public");
    }

    [Fact]
    public void ToString_NeverIncludesCommunityString()
    {
        var command = new ExecuteWorkflowCommand(_workflow.Id, "10.0.0.1", "super-secret-community");

        command.ToString().Should().NotContain("super-secret-community");
        command.ToString().Should().Contain("10.0.0.1");
    }
}

public class PublishWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly PublishWorkflowCommandHandler _handler;

    public PublishWorkflowCommandHandlerTests()
        => _handler = new PublishWorkflowCommandHandler(UnitOfWork, Specification, Parser);

    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _handler.Handle(new PublishWorkflowCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        UnitOfWork.DidNotReceive().Complete();
    }

    [Fact]
    public async Task Handle_WorkflowIsDraft_PublishesAndCompletes()
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf", Status = WorkflowStatus.Draft };
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneShortAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(workflow);

        await _handler.Handle(new PublishWorkflowCommand(workflow.Id), CancellationToken.None);

        workflow.Status.Should().Be(WorkflowStatus.Published);
        Workflows.Received().ReplaceOne(workflow);
        UnitOfWork.Received().Complete();
    }

    [Theory]
    [InlineData(WorkflowStatus.Published)]
    [InlineData(WorkflowStatus.Archived)]
    public async Task Handle_WorkflowNotDraft_ThrowsWorkflowNotEditable(WorkflowStatus status)
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf", Status = status };
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneShortAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(workflow);

        var act = async () => await _handler.Handle(new PublishWorkflowCommand(workflow.Id), CancellationToken.None);

        await act.Should().ThrowAsync<WorkflowNotEditableException>();
        UnitOfWork.DidNotReceive().Complete();
    }
}

public class ArchiveWorkflowCommandHandlerTests : WorkflowHandlerTestBase
{
    readonly ArchiveWorkflowCommandHandler _handler;

    public ArchiveWorkflowCommandHandlerTests()
        => _handler = new ArchiveWorkflowCommandHandler(UnitOfWork, Specification, Parser);

    [Fact]
    public async Task Handle_WorkflowNotFound_ThrowsEntityNotFound()
    {
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _handler.Handle(new ArchiveWorkflowCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        UnitOfWork.DidNotReceive().Complete();
    }

    [Theory]
    [InlineData(WorkflowStatus.Draft)]
    [InlineData(WorkflowStatus.Published)]
    public async Task Handle_WorkflowNotAlreadyArchived_ArchivesAndCompletes(WorkflowStatus status)
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf", Status = status };
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneShortAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(workflow);

        await _handler.Handle(new ArchiveWorkflowCommand(workflow.Id), CancellationToken.None);

        workflow.Status.Should().Be(WorkflowStatus.Archived);
        UnitOfWork.Received().Complete();
    }

    [Fact]
    public async Task Handle_WorkflowAlreadyArchived_ThrowsWorkflowNotEditable()
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid(), Name = "wf", Status = WorkflowStatus.Archived };
        Workflows.AnyByQueryAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(true);
        Workflows.GetOneShortAsync(Arg.Any<ISpecification<WorkflowDefinition>>(), Arg.Any<CancellationToken>()).Returns(workflow);

        var act = async () => await _handler.Handle(new ArchiveWorkflowCommand(workflow.Id), CancellationToken.None);

        await act.Should().ThrowAsync<WorkflowNotEditableException>();
        UnitOfWork.DidNotReceive().Complete();
    }
}
