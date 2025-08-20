namespace Tests.CommonQueryFilterParser;

public class CommonQueryFilterParserTests
{
  private readonly IQueryFilterParser _parser = new Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers.CommonQueryFilterParser();

  [Fact]
  public void ParseFilters_ShouldReturnNull_WhenFiltersIsNull()
  {
    // Act
    var result = _parser.ParseFilters<Person>(null);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public void ParseFilters_ShouldReturnNull_WhenFiltersIsEmpty()
  {
    // Act
    var result = _parser.ParseFilters<Person>([]);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public void ParseFilters_ShouldBuildExpression_ForSingleFilter()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(Person.Age), FilterOperator.GraterThan, 18.ToString())
    };
    var expr = _parser.ParseFilters<Person>(filters)!;

    var compiled = expr.Compile();
    compiled(new Person { Age = 20 }).Should().BeTrue();
    compiled(new Person { Age = 10 }).Should().BeFalse();
  }

  [Fact]
  public void ParseFilters_ShouldCombineWithOr_ForSameProperty()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(Person.Name), FilterOperator.Equals, "Alice"),
      new(nameof(Person.Name), FilterOperator.Equals, "Bob")
    };

    var expr = _parser.ParseFilters<Person>(filters)!;
    var compiled = expr.Compile();

    compiled(new Person { Name = "Alice" }).Should().BeTrue();
    compiled(new Person { Name = "Bob" }).Should().BeTrue();
    compiled(new Person { Name = "Charlie" }).Should().BeFalse();
  }

  [Fact]
  public void ParseFilters_ShouldCombineWithAnd_ForDifferentProperties()
  {
    var filters = new List<FilterDescriptor>
    {
      new(nameof(Person.Name), FilterOperator.Equals, "Alice"),
      new(nameof(Person.Age), FilterOperator.GraterThan, 18.ToString())
    };

    var expr = _parser.ParseFilters<Person>(filters)!;
    var compiled = expr.Compile();

    compiled(new Person { Name = "Alice", Age = 20 }).Should().BeTrue();
    compiled(new Person { Name = "Alice", Age = 15 }).Should().BeFalse();
    compiled(new Person { Name = "Bob", Age = 25 }).Should().BeFalse();
  }

  [Fact]
  public void ParseOrderBy_ShouldReturnNull_WhenPropertyNameIsNullOrEmpty()
  {
    _parser.ParseOrderBy<Person>(null).Should().BeNull();
    _parser.ParseOrderBy<Person>("").Should().BeNull();
    _parser.ParseOrderBy<Person>("   ").Should().BeNull();
  }

  [Fact]
  public void ParseOrderBy_ShouldBuildExpression_ForSimpleProperty()
  {
    var expr = _parser.ParseOrderBy<Person>("Name")!;
    var compiled = expr.Compile();

    compiled(new Person { Name = "Alice" }).Should().Be("Alice");
  }

  [Fact]
  public void ParseOrderBy_ShouldBuildExpression_ForNestedProperty()
  {
    var expr = _parser.ParseOrderBy<Person>("Address.City")!;
    var compiled = expr.Compile();

    var person = new Person
    {
      Address = new() { City = "London" }
    };

    compiled(person).Should().Be("London");
  }
}
