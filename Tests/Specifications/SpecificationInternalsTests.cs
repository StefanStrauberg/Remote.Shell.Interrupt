using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Specification.Specifications;

namespace Tests.Specifications;

public class IncludeChainTests
{
    [Fact]
    public void AddInclude_RegistersBaseEntityAndPropertyTypes()
    {
        var chain = new IncludeChain<Client>();

        chain.AddInclude(c => c.COD);

        chain.Includes.Should().ContainSingle();
        chain.Includes[0].EntityType.Should().Be(typeof(Client));
        chain.Includes[0].PropertyType.Should().Be(typeof(COD));
        chain.Includes[0].Expression.Should().NotBeNull();
    }

    [Fact]
    public void AddThenInclude_AppendsNestedNavigationAfterInclude()
    {
        var chain = new IncludeChain<Client>();

        chain.AddInclude(c => c.COD);
        chain.AddThenInclude<COD, string>(cod => cod.NameCOD);

        chain.Includes.Should().HaveCount(2);
        chain.Includes[1].EntityType.Should().Be(typeof(COD));
        chain.Includes[1].PropertyType.Should().Be(typeof(string));
    }

    [Fact]
    public void AddThenInclude_WithoutInclude_ThrowsInvalidOperationException()
    {
        var chain = new IncludeChain<Client>();

        var act = () => chain.AddThenInclude<COD, string>(cod => cod.NameCOD);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddTypedInclude_RegistersExplicitTypes()
    {
        var chain = new IncludeChain<Client>();

        chain.AddTypedInclude<Client, List<SPRVlan>>(c => c.SPRVlans);

        chain.Includes.Should().ContainSingle();
        chain.Includes[0].EntityType.Should().Be(typeof(Client));
        chain.Includes[0].PropertyType.Should().Be(typeof(List<SPRVlan>));
    }

    [Fact]
    public void Clone_ProducesIndependentCopyWithSameIncludes()
    {
        var chain = new IncludeChain<Client>();
        chain.AddInclude(c => c.COD);

        var clone = (IncludeChain<Client>)chain.Clone();

        clone.Includes.Should().HaveCount(1);
        clone.Includes[0].EntityType.Should().Be(typeof(Client));
        clone.AddInclude(c => c.TfPlan!);
        clone.Includes.Should().HaveCount(2);
        chain.Includes.Should().HaveCount(1);
    }
}

public class GenericSpecificationInternalsTests
{
    private sealed class TestSpec : GenericSpecification<Gate> { }

    [Fact]
    public void AddFilteredInclude_AddsExpressionToFilteredIncludeChains()
    {
        var spec = new TestSpec();

        spec.AddFilteredInclude(g => g.Name.AsEnumerable());

        spec.FilteredIncludeChains.Should().HaveCount(1);
    }

    [Fact]
    public void AddFilteredInclude_NullExpression_ThrowsArgumentNullException()
    {
        var spec = new TestSpec();

        var act = () => spec.AddFilteredInclude<int>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConfigurePagination_EnormousPageNumber_ClampsSkipToIntMax()
    {
        var spec = new TestSpec();

        spec.ConfigurePagination(new PaginationContext(int.MaxValue, 10));

        spec.Skip.Should().Be(int.MaxValue);
        spec.Take.Should().Be(10);
    }

    [Fact]
    public void ConfigurePagination_EnormousPageSize_CapsAtMaxPageSize()
    {
        var spec = new TestSpec();

        spec.ConfigurePagination(new PaginationContext(1, int.MaxValue));

        spec.Take.Should().Be(GenericSpecification<Gate>.MaxPageSize);
    }

    [Fact]
    public void Clone_PreservesCriteriaAndPaginationAndIncludes()
    {
        var spec = new TestSpec();
        spec.ConfigurePagination(new PaginationContext(2, 20));
        spec.AddFilter(g => g.Name == "gw");
        spec.AddInclude(g => g.Name);

        var clone = spec.Clone();

        clone.Should().NotBeSameAs(spec);
        clone.Criterias.Should().NotBeNull();
        clone.Skip.Should().Be(20);
        clone.Take.Should().Be(20);
        clone.IncludeChains.Should().HaveCount(1);
        clone.IncludeChains[0].Includes[0].PropertyType.Should().Be(typeof(string));
    }

    [Fact]
    public void Clone_ModifyingCloneDoesNotAffectOriginal()
    {
        var spec = new TestSpec();
        spec.AddInclude(g => g.Name);
        var originalCount = spec.IncludeChains.Count;

        var clone = spec.Clone();
        clone.AddFilter(g => g.Name == "other");
        clone.AddInclude(g => g.Community);

        spec.IncludeChains.Should().HaveCount(originalCount);
        spec.Criterias.Should().BeNull();
    }

    [Fact]
    public void AddThenInclude_OnSpecification_AttachesToLastChain()
    {
        var spec = new TestSpec();
        spec.AddInclude(g => g.Name);

        var act = () => spec.AddThenInclude<Gate, string>(g => g.Name);

        act.Should().NotThrow();
        spec.IncludeChains.Should().ContainSingle();
        spec.IncludeChains[0].Includes.Should().HaveCount(2);
    }

    [Fact]
    public void AddFilter_CombinesMultipleCriteriaWithAnd()
    {
        var spec = new TestSpec();
        spec.AddFilter(g => g.Name == "gw");
        spec.AddFilter(g => g.Community == "public");

        var compiled = spec.Criterias!.Compile();
        compiled(new Gate { Name = "gw", Community = "public" }).Should().BeTrue();
        compiled(new Gate { Name = "gw", Community = "private" }).Should().BeFalse();
    }
}
