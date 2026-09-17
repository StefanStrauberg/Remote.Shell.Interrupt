using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Queries;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Application.Features.Core;

internal sealed record TestGateByFilterQuery(RequestParameters Parameters)
    : FindEntityByFilterQuery<GateDTO>(Parameters);

/// <summary>
/// Deliberately does not override <c>BuildSpecification</c> or <c>MapToDto</c>, unlike every
/// concrete handler in the codebase - so this test double exercises
/// <see cref="FindEntityByFilterQueryHandler{TEntity,TDto,TQuery}"/>'s own default
/// implementations of those two members, which no production handler currently reaches.
/// </summary>
internal sealed class TestGateByFilterQueryHandler(ISpecification<Gate> specification,
                                                   IQueryFilterParser queryFilterParser,
                                                   IMapper mapper)
    : FindEntityByFilterQueryHandler<Gate, GateDTO, TestGateByFilterQuery>(specification, queryFilterParser, mapper)
{
    public Gate? Entity { get; set; }
    public bool Exists { get; set; } = true;

    protected override Task EnsureEntityExistAsync(ISpecification<Gate> specification, CancellationToken cancellationToken)
    {
        if (!Exists)
            throw new EntityNotFoundException(typeof(Gate), "Id");

        return Task.CompletedTask;
    }

    protected override Task<Gate> FetchEntityAsync(ISpecification<Gate> specification, CancellationToken cancellationToken)
        => Task.FromResult(Entity!);
}

public class FindEntityByFilterQueryHandlerTests
{
    readonly ISpecification<Gate> _specification = CreateSpecificationMock();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper = Substitute.For<IMapper>();
    readonly TestGateByFilterQueryHandler _handler;

    public FindEntityByFilterQueryHandlerTests()
    {
        _handler = new TestGateByFilterQueryHandler(_specification, _parser, _mapper);
    }

    static ISpecification<Gate> CreateSpecificationMock()
    {
        var spec = Substitute.For<ISpecification<Gate>>();
        spec.Clone().Returns(spec);
        spec.AddFilter(Arg.Any<Expression<Func<Gate, bool>>>()).Returns(spec);
        return spec;
    }

    [Fact]
    public async Task Handle_EntityExists_MapsAndReturnsDto()
    {
        var gate = new Gate { Id = Guid.NewGuid(), Name = "gw-1" };
        _handler.Entity = gate;
        var dto = new GateDTO { Name = "gw-1" };
        _mapper.Map<GateDTO>(gate).Returns(dto);

        var result = await _handler.Handle(new TestGateByFilterQuery(new RequestParameters()), CancellationToken.None);

        result.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task Handle_EntityDoesNotExist_ThrowsEntityNotFoundException()
    {
        _handler.Exists = false;

        Func<Task> act = () => _handler.Handle(new TestGateByFilterQuery(new RequestParameters()), CancellationToken.None).AsTask();

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_FiltersProvided_DefaultBuildSpecificationAddsParsedFilter()
    {
        _handler.Entity = new Gate { Id = Guid.NewGuid(), Name = "gw-1" };
        _mapper.Map<GateDTO>(Arg.Any<Gate>()).Returns(new GateDTO());
        var parameters = new RequestParameters
        {
            Filters = [new FilterDescriptor { PropertyPath = nameof(Gate.Name), Operator = FilterOperator.Equals, Value = "gw-1" }]
        };

        await _handler.Handle(new TestGateByFilterQuery(parameters), CancellationToken.None);

        _specification.Received().AddFilter(Arg.Any<Expression<Func<Gate, bool>>>());
    }

    [Fact]
    public async Task Handle_NoFilters_DefaultBuildSpecificationDoesNotAddFilter()
    {
        _handler.Entity = new Gate { Id = Guid.NewGuid(), Name = "gw-1" };
        _mapper.Map<GateDTO>(Arg.Any<Gate>()).Returns(new GateDTO());

        await _handler.Handle(new TestGateByFilterQuery(new RequestParameters()), CancellationToken.None);

        _specification.DidNotReceive().AddFilter(Arg.Any<Expression<Func<Gate, bool>>>());
    }
}
