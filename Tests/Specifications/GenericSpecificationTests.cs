namespace Tests.Specifications;

public class GenericSpecificationTests
{
    private class TestSpec : GenericSpecification<BaseEntity> { }

    [Fact]
    public void AddFilter_NullCriteria_ReturnsThisWithoutChanging()
    {
        var spec = new TestSpec();
        var result = spec.AddFilter(null!);
        result.Should().BeSameAs(spec);
        spec.Criterias.Should().BeNull();
    }

    [Fact]
    public void AddFilter_SingleFilter_SetsCriteria()
    {
        var spec = new TestSpec();
        Expression<Func<BaseEntity, bool>> filter = e => e.Id != Guid.Empty;
        spec.AddFilter(filter);
        spec.Criterias.Should().NotBeNull();
    }

    [Fact]
    public void AddFilter_MultipleFilters_CombinesWithAndAlso()
    {
        var spec = new TestSpec();
        spec.AddFilter(e => e.Id != Guid.Empty);
        spec.AddFilter(e => e.CreatedAt > DateTime.MinValue);
        spec.Criterias.Should().NotBeNull();
    }

    [Fact]
    public void AddOrderBy_Null_ReturnsThisWithoutChanging()
    {
        var spec = new TestSpec();
        spec.AddOrderBy<object>(null!);
        spec.OrderBy.Should().BeNull();
        spec.OrderByDescending.Should().BeNull();
    }

    [Fact]
    public void AddOrderBy_ValidExpression_SetsOrderBy_ClearsOrderByDescending()
    {
        var spec = new TestSpec();
        spec.AddOrderByDescending(e => e.Id);
        spec.AddOrderBy(e => e.CreatedAt);
        spec.OrderBy.Should().NotBeNull();
        spec.OrderByDescending.Should().BeNull();
    }

    [Fact]
    public void AddOrderByDescending_ValidExpression_SetsOrderByDescending_ClearsOrderBy()
    {
        var spec = new TestSpec();
        spec.AddOrderBy(e => e.Id);
        spec.AddOrderByDescending(e => e.CreatedAt);
        spec.OrderByDescending.Should().NotBeNull();
        spec.OrderBy.Should().BeNull();
    }

    [Fact]
    public void ConfigurePagination_ValidPageNumberPageSize_SetsSkipAndTake()
    {
        var spec = new TestSpec();
        var ctx = new PaginationContext(2, 10);
        spec.ConfigurePagination(ctx);
        spec.Skip.Should().Be(10);
        spec.Take.Should().Be(10);
    }

    [Fact]
    public void ConfigurePagination_PageNumberLessThan1_DefaultsTo1()
    {
        var spec = new TestSpec();
        var ctx = new PaginationContext(0, 10);
        spec.ConfigurePagination(ctx);
        spec.Skip.Should().Be(0);
        spec.Take.Should().Be(10);
    }

    [Fact]
    public void ConfigurePagination_PageSizeLessThan1_DefaultsTo10()
    {
        var spec = new TestSpec();
        var ctx = new PaginationContext(1, 0);
        spec.ConfigurePagination(ctx);
        spec.Skip.Should().Be(0);
        spec.Take.Should().Be(10);
    }

    [Fact]
    public void ConfigurePagination_PageSizeExceedsMaxPageSize_CappedAtMax()
    {
        var spec = new TestSpec();
        var ctx = new PaginationContext(1, 5000);
        spec.ConfigurePagination(ctx);
        spec.Take.Should().Be(1000); // MaxPageSize
    }

    [Fact]
    public void ConfigurePagination_NullContext_ThrowsArgumentNullException()
    {
        var spec = new TestSpec();
        Action act = () => spec.ConfigurePagination(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Clone_ReturnsCloneWithSameValues()
    {
        var spec = new TestSpec();
        var ctx = new PaginationContext(3, 20);
        spec.ConfigurePagination(ctx);
        spec.AddOrderBy(e => e.Id);

        var clone = spec.Clone();
        clone.Skip.Should().Be(spec.Skip);
        clone.Take.Should().Be(spec.Take);
        clone.OrderBy.Should().NotBeNull();
    }

    [Fact]
    public void AddInclude_AddsIncludeChain()
    {
        var spec = new GenericSpecification<Gate>();
        spec.AddInclude(g => g.Name);
        spec.IncludeChains.Should().HaveCount(1);
    }

    [Fact]
    public void AddThenInclude_NoPriorInclude_ThrowsInvalidOperationException()
    {
        var spec = new GenericSpecification<Gate>();
        Action act = () => spec.AddThenInclude<Gate, string>(g => g.Name);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddThenInclude_AfterInclude_AddsThenInclude()
    {
        var spec = new GenericSpecification<Port>();
        spec.AddInclude(p => p.InterfaceName);
        Action act = () => spec.AddThenInclude<Port, string>(p => p.InterfaceName);
        act.Should().NotThrow();
    }
}
