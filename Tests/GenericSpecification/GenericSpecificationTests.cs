namespace Tests.GenericSpecification;

public class GenericSpecificationTests
{
  [Fact]
  public void AddFilter_CombinesMultipleCriteriaWithAnd()
  {
    var spec = new GenericSpecification<TestEntity>();
    spec.AddFilter(p => p.Age > 18);
    spec.AddFilter(p => p.Name.StartsWith('J'));

    var result = spec.Criterias;
    result.Should().NotBe(null);


    var compiled = result.Compile();
    var entity1 = new TestEntity { Age = 20, Name = "John" };
    var entity2 = new TestEntity { Age = 18, Name = "Jack" };
    compiled(entity1).Should().BeTrue();
    compiled(entity2).Should().BeFalse();
  }

  [Fact]
  public void AddOrderBy_ShouldSetOrderBy_AndClearOrderByDescending()
  {
    var spec = new GenericSpecification<TestEntity>().AddOrderBy(p => p.Name);

    spec.OrderBy.Should().NotBeNull();
    spec.OrderByDescending.Should().BeNull();
  }

  [Fact]
  public void AddOrderByDescending_ShouldSetOrderByDescending_AndClearOrderBy()
  {
    var spec = new GenericSpecification<TestEntity>().AddOrderByDescending(p => p.Age);

    spec.OrderBy.Should().BeNull();
    spec.OrderByDescending.Should().NotBeNull();
  }

  [Fact]
  public void AddInclude_ShouldAddIncludeChain()
  {
    var spec = new GenericSpecification<TestEntity>().AddInclude(p => p.Children);

    spec.IncludeChains.Should().HaveCount(1);
    spec.IncludeChains[0].Includes.Should().ContainSingle().Which.EntityType.Should().Be<TestEntity>();
  }

  [Fact]
  public void AddThenInclude_ShouldAddToLastChain()
  {
    var spec = new GenericSpecification<TestEntity>().AddInclude(p => p.Children)
                                                 .AddThenInclude<ChildEntity, string>(a => a.ChildName);

    spec.IncludeChains.Should().HaveCount(1);
    spec.IncludeChains[0].Includes.Should().HaveCount(2);
  }

  [Fact]
  public void AddThenInclude_ShouldThrow_WhenNoIncludeExists()
  {
    var spec = new GenericSpecification<TestEntity>();
    Action act = () => spec.AddThenInclude<ChildEntity, string>(n => n.ChildName);

    act.Should().Throw<InvalidOperationException>().WithMessage("No Include found to apply ThenInclude");
  }

  [Fact]
  public void AddFilteredInclude_ShouldAddLambda()
  {
    var spec = new GenericSpecification<TestEntity>().AddFilteredInclude(p => p.Children.Where(n => n.ChildName == "John"));

    spec.FilteredIncludeChains.Should().HaveCount(1);
  }

  [Fact]
  public void ConfigurePagination_ShouldSetSkipAndTake()
  {
    var spec = new GenericSpecification<TestEntity>().ConfigurePagination(new PaginationContext(PageNumber: 2, PageSize: 5));

    spec.Skip.Should().Be(5);
    spec.Take.Should().Be(5);
  }

  [Fact]
  public void ConfigurePagination_ShouldDefaultInvalidValues()
  {
    var spec = new GenericSpecification<TestEntity>().ConfigurePagination(new PaginationContext(PageNumber: 0, PageSize: 0));

    spec.Skip.Should().Be(0);
    spec.Take.Should().Be(10);
  }

  [Fact]
  public void Clone_ShouldCopyAllProperties()
  {
    var spec = new GenericSpecification<TestEntity>().AddFilter(p => p.Age > 18)
                                                 .AddOrderBy(p => p.Name)
                                                 .AddInclude(p => p.Children)
                                                 .ConfigurePagination(new PaginationContext(PageNumber: 1, PageSize: 5));

    var clone = spec.Clone();

    clone.Skip.Should().Be(spec.Skip);
    clone.Take.Should().Be(spec.Take);
    clone.OrderBy.Should().NotBeNull();
    clone.IncludeChains.Should().HaveCount(1);
    clone.Criterias.Should().NotBeNull();
  }
}
