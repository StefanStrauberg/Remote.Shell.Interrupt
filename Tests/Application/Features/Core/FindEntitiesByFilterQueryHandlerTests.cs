using AutoMapper;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Core.Queries;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Application.Features.Core;

internal sealed record TestGateFilterQuery(RequestParameters Parameters)
    : FindEntitiesByFilterQuery<GateDTO>(Parameters);

internal sealed class TestGateFilterQueryHandler(ISpecification<Gate> specification,
                                                IQueryFilterParser queryFilterParser,
                                                IMapper mapper)
    : FindEntitiesByFilterQueryHandler<Gate, GateDTO, TestGateFilterQuery>(specification, queryFilterParser, mapper)
{
    public List<Gate> Source { get; set; } = [];
    public int CountValue { get; set; }
    public int CountCalls { get; private set; }

    protected override Task<IEnumerable<Gate>> FetchEntitiesAsync(ISpecification<Gate> specification,
                                                                  CancellationToken cancellationToken)
        => Task.FromResult<IEnumerable<Gate>>(Source);

    protected override Task<int> CountResultsAsync(ISpecification<Gate> specification,
                                                   CancellationToken cancellationToken)
    {
        CountCalls++;
        return Task.FromResult(CountValue);
    }
}

public class FindEntitiesByFilterQueryHandlerTests
{
    readonly ISpecification<Gate> _specification = CreateSpecificationMock();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper = Substitute.For<IMapper>();
    readonly TestGateFilterQueryHandler _handler;

    public FindEntitiesByFilterQueryHandlerTests()
    {
        _handler = new TestGateFilterQueryHandler(_specification, _parser, _mapper);
    }

    static ISpecification<Gate> CreateSpecificationMock()
    {
        var spec = Substitute.For<ISpecification<Gate>>();
        spec.Clone().Returns(spec);
        spec.AddFilter(Arg.Any<Expression<Func<Gate, bool>>>()).Returns(spec);
        spec.AddOrderBy(Arg.Any<Expression<Func<Gate, object>>>()).Returns(spec);
        spec.AddOrderByDescending(Arg.Any<Expression<Func<Gate, object>>>()).Returns(spec);
        return spec;
    }

    static List<Gate> ThreeGates() =>
    [
        new Gate { Id = Guid.NewGuid(), Name = "gw-1" },
        new Gate { Id = Guid.NewGuid(), Name = "gw-2" },
        new Gate { Id = Guid.NewGuid(), Name = "gw-3" }
    ];

    void SetupMapperReturning(int count)
    {
        var dtos = Enumerable.Range(0, count).Select(_ => new GateDTO()).ToList();
        _mapper.Map<IEnumerable<GateDTO>>(Arg.Any<object>()).Returns(dtos);
    }

    [Fact]
    public async Task Handle_PaginatedRequest_ConfiguresPaginationAndCounts()
    {
        _handler.Source = ThreeGates();
        _handler.CountValue = 7;
        SetupMapperReturning(3);
        var parameters = new RequestParameters { PageNumber = 2, PageSize = 2 };
        PaginationContext configured = new(0, 0);
        _specification.ConfigurePagination(Arg.Do<PaginationContext>(ctx => configured = ctx)).Returns(_specification);

        var result = await _handler.Handle(new TestGateFilterQuery(parameters), CancellationToken.None);

        configured.PageNumber.Should().Be(2);
        configured.PageSize.Should().Be(2);
        _handler.CountCalls.Should().Be(1);
        result.TotalCount.Should().Be(7);
        result.TotalPages.Should().Be(4);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.HasPrevious.Should().BeTrue();
        result.HasNext.Should().BeTrue();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_NonPaginatedRequest_SkipsCountQueryAndReportsSinglePage()
    {
        _handler.Source = ThreeGates();
        SetupMapperReturning(3);

        var result = await _handler.Handle(new TestGateFilterQuery(new RequestParameters()), CancellationToken.None);

        _specification.DidNotReceive().ConfigurePagination(Arg.Any<PaginationContext>());
        _handler.CountCalls.Should().Be(0);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(3);
        result.HasPrevious.Should().BeFalse();
        result.HasNext.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedListWithoutCounting()
    {
        _handler.Source = [];
        SetupMapperReturning(0);

        var result = await _handler.Handle(
            new TestGateFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 }),
            CancellationToken.None);

        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        _handler.CountCalls.Should().Be(0);
    }

    [Fact]
    public async Task Handle_OrderBySet_AppliesAscendingSortToSpecification()
    {
        _handler.Source = ThreeGates();
        SetupMapperReturning(3);
        var parameters = new RequestParameters { OrderBy = nameof(Gate.Name) };

        await _handler.Handle(new TestGateFilterQuery(parameters), CancellationToken.None);

        _specification.Received().AddOrderBy(Arg.Any<Expression<Func<Gate, object>>>());
        _specification.DidNotReceive().AddOrderByDescending(Arg.Any<Expression<Func<Gate, object>>>());
    }

    [Fact]
    public async Task Handle_OrderByDescendingSet_AppliesDescendingSortToSpecification()
    {
        _handler.Source = ThreeGates();
        SetupMapperReturning(3);
        var parameters = new RequestParameters { OrderBy = nameof(Gate.Name), OrderByDescending = true };

        await _handler.Handle(new TestGateFilterQuery(parameters), CancellationToken.None);

        _specification.Received().AddOrderByDescending(Arg.Any<Expression<Func<Gate, object>>>());
        _specification.DidNotReceive().AddOrderBy(Arg.Any<Expression<Func<Gate, object>>>());
    }

    [Fact]
    public async Task Handle_FiltersProvided_AddsParsedFilterToSpecification()
    {
        _handler.Source = ThreeGates();
        SetupMapperReturning(3);
        var parameters = new RequestParameters
        {
            Filters = [new FilterDescriptor { PropertyPath = nameof(Gate.Name), Operator = FilterOperator.Equals, Value = "gw-1" }]
        };

        await _handler.Handle(new TestGateFilterQuery(parameters), CancellationToken.None);

        _specification.Received().AddFilter(Arg.Any<Expression<Func<Gate, bool>>>());
    }
}
