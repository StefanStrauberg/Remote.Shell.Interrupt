namespace Tests.GenericSpecification;

public class GenericSpecificationTests
{
  [Fact]
  public void AddFilter_ShouldCombineExpressions()
  {
    var spec = new GenericSpecification<Person>();
    spec.AddFilter(p => p.Age > 18)
        .AddFilter(p => p.Name.StartsWith('A'));

    var expr = spec.Criterias!;
    var compiled = expr.Compile();

    compiled(new Person { Age = 20, Name = "Alex" }).Should().BeTrue();
    compiled(new Person { Age = 20, Name = "Bob" }).Should().BeFalse();
  }

  [Fact]
  public void AddOrderBy_ShouldSetOrderBy_AndClearOrderByDescending()
  {
    var spec = new GenericSpecification<Person>().AddOrderBy(p => p.Name);

    spec.OrderBy.Should().NotBeNull();
    spec.OrderByDescending.Should().BeNull();
  }

  [Fact]
  public void AddOrderByDescending_ShouldSetOrderByDescending_AndClearOrderBy()
  {
    var spec = new GenericSpecification<Person>().AddOrderByDescending(p => p.Age);

    spec.OrderBy.Should().BeNull();
    spec.OrderByDescending.Should().NotBeNull();
  }

  [Fact]
  public void AddInclude_ShouldAddIncludeChain()
  {
    var spec = new GenericSpecification<Person>().AddInclude(p => p.NickNames);

    spec.IncludeChains.Should().HaveCount(1);
    spec.IncludeChains[0].Includes.Should().ContainSingle().Which.EntityType.Should().Be<Person>();
  }

  [Fact]
  public void AddThenInclude_ShouldAddToLastChain()
  {
    var spec = new GenericSpecification<Person>().AddInclude(p => p.NickNames)
                                                 .AddThenInclude<NickName, string>(a => a.Nick);

    spec.IncludeChains.Should().HaveCount(1);
    spec.IncludeChains[0].Includes.Should().HaveCount(2);
  }

  [Fact]
  public void AddThenInclude_ShouldThrow_WhenNoIncludeExists()
  {
    var spec = new GenericSpecification<Person>();
    Action act = () => spec.AddThenInclude<NickName, string>(n => n.Nick);

    act.Should().Throw<InvalidOperationException>().WithMessage("No Include found to apply ThenInclude");
  }

  [Fact]
  public void AddFilteredInclude_ShouldAddLambda()
  {
    var spec = new GenericSpecification<Person>().AddFilteredInclude(p => p.NickNames.Where(n => n.Nick == "John"));

    spec.FilteredIncludeChains.Should().HaveCount(1);
  }

  [Fact]
  public void ConfigurePagination_ShouldSetSkipAndTake()
  {
    var spec = new GenericSpecification<Person>().ConfigurePagination(new PaginationContext(PageNumber: 2, PageSize: 5));

    spec.Skip.Should().Be(5);
    spec.Take.Should().Be(5);
  }

  [Fact]
  public void ConfigurePagination_ShouldDefaultInvalidValues()
  {
    var spec = new GenericSpecification<Person>().ConfigurePagination(new PaginationContext(PageNumber: 0, PageSize: 0));

    spec.Skip.Should().Be(0);
    spec.Take.Should().Be(10);
  }

  [Fact]
  public void Clone_ShouldCopyAllProperties()
  {
    var spec = new GenericSpecification<Person>().AddFilter(p => p.Age > 18)
                                                 .AddOrderBy(p => p.Name)
                                                 .AddInclude(p => p.NickNames)
                                                 .ConfigurePagination(new PaginationContext(PageNumber: 1, PageSize: 5));

    var clone = spec.Clone();

    clone.Skip.Should().Be(spec.Skip);
    clone.Take.Should().Be(spec.Take);
    clone.OrderBy.Should().NotBeNull();
    clone.IncludeChains.Should().HaveCount(1);
    clone.Criterias.Should().NotBeNull();
  }
}
