namespace Tests;

public class FilterDescriptorExtensionsTests
{
  // Test entities
  class TestEntity
  {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public decimal Salary { get; set; }
    public bool IsActive { get; set; }
    public TestNestedEntity Nested { get; set; } = new();
    public List<TestNestedEntity> NestedList { get; set; } = [];
    public Guid UniqueId { get; set; }
    public TestEnum Status { get; set; }
  }

  class TestNestedEntity
  {
    public string NestedName { get; set; } = string.Empty;
    public int NestedValue { get; set; }
    public TestDeeplyNestedEntity DeeplyNested { get; set; } = new();
  }

  class TestDeeplyNestedEntity
  {
    public string DeepValue { get; set; } = string.Empty;
  }

  enum TestEnum
  {
    Pending,
    Active,
    Inactive
  }

  // 1. Basic filter tests
  #region 
  [Fact]
  public void ToExpression_NullFilter_ThrowsArgumentNullException()
  {
    // Arrange

    // Act
    Action act = () => FilterDescriptorExtensions.ToExpression<TestEntity>(null!);

    // Assert
    act.Should().Throw<ArgumentNullException>().WithMessage("Filter descriptor cannot be null when building an expression. (Parameter 'filter')");
  }

  [Theory]
  [InlineData(FilterOperator.Equals, "25", true)]
  [InlineData(FilterOperator.Equals, "30", false)]
  [InlineData(FilterOperator.NotEquals, "25", false)]
  [InlineData(FilterOperator.NotEquals, "30", true)]
  [InlineData(FilterOperator.GraterThan, "20", true)]
  [InlineData(FilterOperator.GraterThan, "25", false)]
  [InlineData(FilterOperator.LessThan, "30", true)]
  [InlineData(FilterOperator.LessThan, "25", false)]
  public void ToExpression_IntProperty_WorksCorrectly(FilterOperator op, string value, bool expected)
  {
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Age),
      Operator = op,
      Value = value
    };

    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();
    var entity = new TestEntity { Age = 25 };

    compiled(entity).Should().Be(expected);
  }
  #endregion

  // 2. String operation tests
  #region 
  [Theory]
  [InlineData(FilterOperator.Equals, "Test", false)]
  [InlineData(FilterOperator.Equals, "test", false)]
  [InlineData(FilterOperator.Equals, "Other", false)]
  [InlineData(FilterOperator.Equals, "", false)]

  [InlineData(FilterOperator.NotEquals, "Test", true)]
  [InlineData(FilterOperator.NotEquals, "test", true)]
  [InlineData(FilterOperator.NotEquals, "Other", true)]
  [InlineData(FilterOperator.NotEquals, "", true)]

  [InlineData(FilterOperator.Contains, "es", true)]
  [InlineData(FilterOperator.Contains, "ES", false)]
  [InlineData(FilterOperator.Contains, "xyz", false)]
  [InlineData(FilterOperator.Contains, "T", true)]
  [InlineData(FilterOperator.Contains, "t", true)]
  [InlineData(FilterOperator.Contains, "", true)]

  [InlineData(FilterOperator.Word, "Test", true)]
  [InlineData(FilterOperator.Word, "test", true)]
  [InlineData(FilterOperator.Word, "Tes", false)]
  [InlineData(FilterOperator.Word, "Testing", false)]
  [InlineData(FilterOperator.Word, "is", false)]
  [InlineData(FilterOperator.Word, "a", false)]
  [InlineData(FilterOperator.Word, "Unit", true)]
  [InlineData(FilterOperator.Word, "", false)]

  [InlineData(FilterOperator.StartsWith, "Te", false)]
  [InlineData(FilterOperator.StartsWith, "te", false)]
  [InlineData(FilterOperator.StartsWith, "Un", true)]
  [InlineData(FilterOperator.StartsWith, "Test", false)]
  [InlineData(FilterOperator.StartsWith, "Test Unit", false)]
  [InlineData(FilterOperator.StartsWith, "", true)]

  [InlineData(FilterOperator.EndsWith, "st", true)]
  [InlineData(FilterOperator.EndsWith, "ST", false)]
  [InlineData(FilterOperator.EndsWith, "nit", false)]
  [InlineData(FilterOperator.EndsWith, "Unit Test", true)]
  [InlineData(FilterOperator.EndsWith, "LongerThanTest", false)]
  [InlineData(FilterOperator.EndsWith, "", true)]
  public void ToExpression_StringProperty_WorksCorrectly(FilterOperator op, string value, bool expected)
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Name),
      Operator = op,
      Value = value
    };
    var testableString = "Unit Test";

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();
    var entity = new TestEntity { Name = testableString };

    // Assert
    compiled(entity).Should().Be(expected, $"operation={op}, value={value} expected={expected} in the string={testableString}");
  }

  [Theory]
  [InlineData("Hello World", "World", true)]
  [InlineData("Hello-World", "World", true)]
  [InlineData("Hello.World", "World", true)]
  [InlineData("Hello,World", "World", true)]
  [InlineData("Hello!World", "World", true)]
  [InlineData("Hello?World", "World", true)]
  [InlineData("Hello;World", "World", true)]
  [InlineData("Hello:World", "World", true)]
  [InlineData("Hello\"World", "World", true)]
  [InlineData("Hello'World", "World", true)]
  [InlineData("Hello(World", "World", true)]
  [InlineData("Hello)World", "World", true)]
  [InlineData("HelloWorld", "World", false)]
  [InlineData("HelloWorldTest", "World", false)]
  public void ToExpression_WordOperator_WithDifferentSeparators(string testString, string searchWord, bool expected)
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Description),
      Operator = FilterOperator.Word,
      Value = searchWord
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();
    var entity = new TestEntity { Description = testString };

    // Assert
    compiled(entity).Should().Be(expected, $"operation={FilterOperator.Word}, value={searchWord} expected={expected} in the string={testString}");
  }

  [Theory]
  [InlineData(FilterOperator.Contains)]
  [InlineData(FilterOperator.Word)]
  [InlineData(FilterOperator.StartsWith)]
  [InlineData(FilterOperator.EndsWith)]
  public void ToExpression_StringOperatorOnNonStringProperty_ThrowsException(FilterOperator op)
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Age),
      Operator = op,
      Value = "test"
    };

    // Act
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<InvalidOperationException>()
                .WithMessage($"Operator {op} can only be used with string properties. Property type: {typeof(TestEntity).GetProperty(nameof(TestEntity.Age))!.PropertyType.Name}");
  }
  #endregion

  // 3. Nested property tests
  #region 
  [Fact]
  public void ToExpression_NestedProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.Nested)}.{nameof(TestEntity.Nested.NestedName)}",
      Operator = FilterOperator.Equals,
      Value = "TestNested"
    };
    var entity = new TestEntity
    {
      Nested = new TestNestedEntity { NestedName = "TestNested" }
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity).Should().BeTrue("the nested property value matches the filter condition");
  }

  [Fact]
  public void ToExpression_DeeplyNestedProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.Nested)}.{nameof(TestEntity.Nested.DeeplyNested)}.{nameof(TestEntity.Nested.DeeplyNested.DeepValue)}",
      Operator = FilterOperator.Equals,
      Value = "DeepTest"
    };
    var entity = new TestEntity
    {
      Nested = new TestNestedEntity
      {
        DeeplyNested = new TestDeeplyNestedEntity { DeepValue = "DeepTest" }
      }
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity).Should().BeTrue("the deeply nested property value matches the filter condition");
  }
  #endregion

  // 4. Collection property tests
  #region 
  [Fact]
  public void ToExpression_CollectionProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.NestedList)}.{nameof(TestNestedEntity.NestedValue)}",
      Operator = FilterOperator.Equals,
      Value = "42"
    };
    var entity = new TestEntity
    {
      NestedList =
      [
        new() { NestedValue = 10 },
        new() { NestedValue = 42 },
        new() { NestedValue = 99 }
      ]
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity).Should().BeTrue("at least one item in the collection matches the filter condition");
  }

  [Fact]
  public void ToExpression_CollectionPropertyWithNoMatch_ReturnsFalse()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.NestedList)}.{nameof(TestNestedEntity.NestedValue)}",
      Operator = FilterOperator.Equals,
      Value = "100"
    };
    var entity = new TestEntity
    {
      NestedList =
      [
        new() { NestedValue = 10 },
        new() { NestedValue = 42 }
      ]
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity).Should().BeFalse("non of the items in the collection matches the filter condition");
  }

  [Fact]
  public void ToExpression_CollectionPropertyWithNestedPath_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.NestedList)}.{nameof(TestNestedEntity.DeeplyNested)}.{nameof(TestDeeplyNestedEntity.DeepValue)}",
      Operator = FilterOperator.Equals,
      Value = "DeepTest"
    };
    var entity = new TestEntity
    {
      NestedList =
      [
        new() { DeeplyNested = new TestDeeplyNestedEntity { DeepValue = "OtherValue" } },
        new() { DeeplyNested = new TestDeeplyNestedEntity { DeepValue = "DeepTest" } }
      ]
    };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity).Should().BeTrue("at least one deep item in the collection matches the filter condition");
  }
  #endregion

  // 5. In operator tests
  #region 
  [Fact]
  public void ToExpression_InOperator_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Age),
      Operator = FilterOperator.In,
      Value = "25,30,35"
    };
    var entity1 = new TestEntity { Age = 25 };
    var entity2 = new TestEntity { Age = 30 };
    var entity3 = new TestEntity { Age = 35 };
    var entity4 = new TestEntity { Age = 40 };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"{entity1.Age} is in the filter list");
    compiled(entity2).Should().BeTrue($"{entity2.Age} is in the filter list");
    compiled(entity3).Should().BeTrue($"{entity3.Age} is in the filter list");
    compiled(entity4).Should().BeFalse($"{entity4.Age} is not in the filter list");
  }

  [Fact]
  public void ToExpression_InOperatorWithStrings_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Name),
      Operator = FilterOperator.In,
      Value = "John,Jane,Test"
    };
    var entity1 = new TestEntity { Name = "John" };
    var entity2 = new TestEntity { Name = "Test" };
    var entity3 = new TestEntity { Name = "Other" };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue("the word John is in the filter list");
    compiled(entity2).Should().BeTrue("the word Test is in the filter list");
    compiled(entity3).Should().BeFalse("the word Other is in the filter list");
  }
  #endregion

  // 6. Different data type tests
  #region 
  [Fact]
  public void ToExpression_BooleanProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.IsActive),
      Operator = FilterOperator.Equals,
      Value = true.ToString()
    };
    var entity1 = new TestEntity { IsActive = true };
    var entity2 = new TestEntity { IsActive = false };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"the property {nameof(TestEntity.IsActive)} is {entity1.IsActive}");
    compiled(entity2).Should().BeFalse($"the property {nameof(TestEntity.IsActive)} is {entity2.IsActive}");
  }

  [Fact]
  public void ToExpression_DateTimeProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.BirthDate),
      Operator = FilterOperator.Equals,
      Value = "2020-01-01"
    };
    var entity1 = new TestEntity { BirthDate = new DateTime(2020, 1, 1) };
    var entity2 = new TestEntity { BirthDate = new DateTime(2020, 1, 2) };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"the property {nameof(TestEntity.BirthDate)} is {entity1.BirthDate}");
    compiled(entity2).Should().BeFalse($"the property {nameof(TestEntity.BirthDate)} is {entity2.BirthDate}");
  }

  [Fact]
  public void ToExpression_DecimalProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Salary),
      Operator = FilterOperator.GraterThan,
      Value = "1000.50"
    };
    var entity1 = new TestEntity { Salary = 1500.75m };
    var entity2 = new TestEntity { Salary = 900.25m };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"the field {nameof(TestEntity.Salary)} is {entity1.Salary}");
    compiled(entity2).Should().BeFalse($"the field {nameof(TestEntity.Salary)} is {entity2.Salary}");
  }

  [Fact]
  public void ToExpression_GuidProperty_WorksCorrectly()
  {
    // Arrange
    var testGuid = Guid.NewGuid();
    var otherGuid = Guid.NewGuid();
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.UniqueId),
      Operator = FilterOperator.Equals,
      Value = testGuid.ToString()
    };
    var entity1 = new TestEntity { UniqueId = testGuid };
    var entity2 = new TestEntity { UniqueId = otherGuid };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"the filed {nameof(TestEntity.UniqueId)} is {testGuid}");
    compiled(entity2).Should().BeFalse($"the filed {nameof(TestEntity.UniqueId)} is {otherGuid}");
  }

  [Fact]
  public void ToExpression_EnumProperty_WorksCorrectly()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Status),
      Operator = FilterOperator.Equals,
      Value = TestEnum.Active.ToString()
    };
    var entity1 = new TestEntity { Status = TestEnum.Active };
    var entity2 = new TestEntity { Status = TestEnum.Pending };

    // Act
    var expression = filter.ToExpression<TestEntity>();
    var compiled = expression.Compile();

    // Assert
    compiled(entity1).Should().BeTrue($"the field {nameof(TestEntity.Status)} is {TestEnum.Active}");
    compiled(entity2).Should().BeFalse($"the field {nameof(TestEntity.Status)} is {TestEnum.Pending}");
  }
  #endregion

  // 7. Error handling tests
  #region 
  [Fact]
  public void ToExpression_InvalidPropertyPath_ThrowsException()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = "NonExistentProperty",
      Operator = FilterOperator.Equals,
      Value = "test"
    };

    // Act 
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void ToExpression_InvalidNestedPropertyPath_ThrowsException()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.Nested)}.NonExistentProperty",
      Operator = FilterOperator.Equals,
      Value = "test"
    };

    // Act
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void ToExpression_InvalidCollectionPropertyPath_ThrowsException()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = $"{nameof(TestEntity.NestedList)}.NonExistentProperty",
      Operator = FilterOperator.Equals,
      Value = "test"
    };

    // Act
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void ToExpression_UnsupportedOperator_ThrowsException()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Name),
      Operator = (FilterOperator)999, // Invalid operator
      Value = "test"
    };

    // Act
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<NotImplementedException>();
  }

  [Fact]
  public void ToExpression_InvalidValueConversion_ThrowsException()
  {
    // Arrange
    var filter = new FilterDescriptor
    {
      PropertyPath = nameof(TestEntity.Age),
      Operator = FilterOperator.Equals,
      Value = "not-an-integer"
    };

    // Act 
    Action act = () => filter.ToExpression<TestEntity>();

    // Assert
    act.Should().Throw<InvalidOperationException>();
  }
  #endregion
}