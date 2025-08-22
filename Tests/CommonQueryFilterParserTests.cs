namespace Tests;

public class CommonQueryFilterParserTests
{
  // Test entities
  public class TestEntity
  {
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
    public TestNestedEntity Nested { get; set; } = new();
    public List<TestNestedEntity> NestedList { get; set; } = [];
  }

  public class TestNestedEntity
  {
    public string NestedName { get; set; } = string.Empty;
    public int NestedValue { get; set; }
  }

  private readonly IQueryFilterParser _parser = new CommonQueryFilterParser();

  // 1. ParseFilters tests
  #region 
  [Fact]
  public void ParseFilters_NullFilters_ReturnsNull()
  {
    // Arrange
    //Act
    var result = _parser.ParseFilters<TestEntity>(null);
    // Assert
    result.Should().BeNull("incoming argument filters is null");
  }

  [Fact]
  public void ParseFilters_EmptyFilters_ReturnsNull()
  {
    // Arrange
    var filters = new List<FilterDescriptor>();
    // Act
    var result = _parser.ParseFilters<TestEntity>(filters);
    // Assert
    result.Should().BeNull("incoming argument filters is empty");
  }

  [Fact]
  public void ParseFilters_SingleFilter_WorksCorrectly()
  {
    // Arrange
    var filters = new List<FilterDescriptor>
    {
      new()
      {
        PropertyPath = nameof(TestEntity.Name),
        Operator = FilterOperator.Equals,
        Value = "Test"
      }
    };
    var matchingEntity = new TestEntity { Name = "Test" };
    var nonMatchingEntity = new TestEntity { Name = "Other" };

    // Act
    var expression = _parser.ParseFilters<TestEntity>(filters);
    var compiled = expression!.Compile();


    // Assert
    expression.Should().NotBeNull();
    compiled(matchingEntity).Should().BeTrue();
    compiled(nonMatchingEntity).Should().BeFalse();
  }
  #endregion
}
