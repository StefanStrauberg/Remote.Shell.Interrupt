namespace Tests.FilterDescriptorExtensions;

public class FilterDescriptorExtensionsTests
{
  [Fact]
  public void Equals_Filter_Should_Work()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Name),
      Operator = FilterOperator.Equals,
      Value = "John"
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Name = "John" });

    result.Should().BeTrue();
  }

  [Fact]
  public void NotEquals_Filter_Should_Work()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Age),
      Operator = FilterOperator.NotEquals,
      Value = 30.ToString()
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Age = 25 });

    result.Should().BeTrue();
  }

  [Fact]
  public void GreaterThan_Filter_Should_Work()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Age),
      Operator = FilterOperator.GraterThan,
      Value = 18.ToString()
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Age = 25 });

    result.Should().BeTrue();
  }

  [Fact]
  public void Contains_Filter_Should_Work_On_String()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Name),
      Operator = FilterOperator.Contains,
      Value = "oh"
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Name = "John" });

    result.Should().BeTrue();
  }

  [Fact]
  public void In_Filter_Should_Work()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Age),
      Operator = FilterOperator.In,
      Value = "18,25,30"
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Age = 25 });

    result.Should().BeTrue();
  }

  [Fact]
  public void Collection_Filter_Should_Work_With_Any()
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = "Nicknames.Nick",
      Operator = FilterOperator.Equals,
      Value = "Doe"
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var person = new Person
    {
      NickNames =
      [
        new() { Nick = "John" },
        new() { Nick = "Doe" }
      ]
    };

    var result = func(person);

    result.Should().BeTrue();
  }
  
  [Fact]
  public void Guid_Filter_Should_Work()
  {
    var guid = Guid.NewGuid();
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(Person.Id),
      Operator = FilterOperator.Equals,
      Value = guid.ToString()
    };

    var expr = filter.ToExpression<Person>();
    var func = expr.Compile();

    var result = func(new Person { Id = guid });

    result.Should().BeTrue();
  }
}
