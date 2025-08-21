namespace Tests.CommonQueryFilterParser;

public class CommonQueryFilterParserTests
{
  private readonly IQueryFilterParser _parser = new Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers.CommonQueryFilterParser();

  [Fact]
  public void ParseFilters_ShouldReturnNull_WhenFiltersIsNull()
  {
    // Act
    var result = _parser.ParseFilters<TestEntity>(null);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public void ParseFilters_ShouldReturnNull_WhenFiltersIsEmpty()
  {
    // Act
    var result = _parser.ParseFilters<TestEntity>([]);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public void ParseFilters_ShouldBuildExpression_ForSingleFilter()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(TestEntity.Age), FilterOperator.GraterThan, 18.ToString())
    };
    var expr = _parser.ParseFilters<TestEntity>(filters)!;

    var compiled = expr.Compile();
    compiled(new TestEntity { Age = 20 }).Should().BeTrue();
    compiled(new TestEntity { Age = 10 }).Should().BeFalse();
  }

  [Fact]
  public void ParseFilters_ShouldCombineWithOr_ForSameProperty()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(TestEntity.Name), FilterOperator.Equals, "Alice"),
      new(nameof(TestEntity.Name), FilterOperator.Equals, "Bob")
    };

    var expr = _parser.ParseFilters<TestEntity>(filters)!;
    var compiled = expr.Compile();

    compiled(new TestEntity { Name = "Alice" }).Should().BeTrue();
    compiled(new TestEntity { Name = "Bob" }).Should().BeTrue();
    compiled(new TestEntity { Name = "Charlie" }).Should().BeFalse();
  }

  [Fact]
  public void ParseFilters_ShouldCombineWithAnd_ForDifferentProperties()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(TestEntity.Name), FilterOperator.Equals, "Alice"),
      new(nameof(TestEntity.Age), FilterOperator.GraterThan, 18.ToString())
    };

    var expr = _parser.ParseFilters<TestEntity>(filters)!;
    var compiled = expr.Compile();

    compiled(new TestEntity { Name = "Alice", Age = 20 }).Should().BeTrue();
    compiled(new TestEntity { Name = "Alice", Age = 15 }).Should().BeFalse();
    compiled(new TestEntity { Name = "Bob", Age = 25 }).Should().BeFalse();
  }

  [Fact]
  public void ParseOrderBy_ShouldReturnNull_WhenPropertyNameIsNullOrEmpty()
  {
    _parser.ParseOrderBy<TestEntity>(null).Should().BeNull();
    _parser.ParseOrderBy<TestEntity>("").Should().BeNull();
    _parser.ParseOrderBy<TestEntity>("   ").Should().BeNull();
  }

  [Fact]
  public void ParseOrderBy_ShouldBuildExpression_ForSimpleProperty()
  {
    var expr = _parser.ParseOrderBy<TestEntity>("Name")!;
    var compiled = expr.Compile();

    compiled(new TestEntity { Name = "Alice" }).Should().Be("Alice");
  }

  [Fact]
  public void ParseOrderBy_ShouldBuildExpression_ForNestedProperty()
  {
    var expr = _parser.ParseOrderBy<TestEntity>("Address.City")!;
    var compiled = expr.Compile();

    var person = new TestEntity
    {
      Address = new() { City = "London" }
    };

    compiled(person).Should().Be("London");
  }
}
