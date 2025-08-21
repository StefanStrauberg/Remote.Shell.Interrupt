using System.Reflection;

namespace Tests.GenericSpecification;

public class GenericSpecificationTests
{
  class TestEntity : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<ChildEntity> Children { get; set; } = [];
  }

  class ChildEntity : BaseEntity
  {
    public string Name { get; set; } = string.Empty;
    public List<ToyEntity> Toys { get; set; } = [];
  }

  class ToyEntity : BaseEntity
  {
    public string ToyName { get; set; } = string.Empty;
  }

  // Helper for extracting property name from Expression
  static string GetIncludePropertyName(IIncludeChain<TestEntity> chain)
    => chain.Includes[0].Expression.ToString().Split('.').Last();

  static string GetPropertyNameFromExpression<T>(Expression<Func<T, object>> expression)
  {
    if (expression.Body is MemberExpression member)
      return member.Member.Name;

    if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
      return unaryMember.Member.Name;

    throw new ArgumentException("Invalid expression");
  }

  // Fitlering tests
  [Fact]
  public void AddFilter_CombinesMultipleCriteriaWithAnd()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var id = Guid.NewGuid();
    var entity = new TestEntity { Id = id, Name = "Test" };

    // Act
    spec.AddFilter(e => e.Id == id);
    spec.AddFilter(e => e.Name == "Test");

    // Assert
    spec.Criterias.Should().NotBeNull("filters should be combined into a single expression");

    var compiled = spec.Criterias.Compile();
    compiled(entity).Should().BeTrue("entity matches all combined filter criteria");
  }

  [Fact]
  public void AddFilter_WithNullCriteria_DoesNothing()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var id = Guid.NewGuid();
    var entity = new TestEntity { Id = id };

    // Act
    spec.AddFilter(e => e.Id == id);
    spec.AddFilter(null!);
    var compiled = spec.Criterias!.Compile();

    // Assert
    spec.Criterias.Should().NotBeNull("a valid filter was added before the null input");
    compiled(entity).Should().BeTrue("the original filter should remain unaffected");
  }

  [Fact]
  public void AddFilter_WithMultipleCriteria_CombinesWithAndLogic()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var id = Guid.NewGuid();
    var matchingEntity = new TestEntity { Id = id, Name = "Test" };
    var nonMatchingEntity1 = new TestEntity { Id = id, Name = "Other" };
    var nonMatchingEntity2 = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };

    // Act
    spec.AddFilter(e => e.Id == id);
    spec.AddFilter(e => e.Name == "Test");
    var compiled = spec.Criterias!.Compile();


    // Assert
    compiled(matchingEntity).Should().BeTrue("entity matches both filter criteria");
    compiled(nonMatchingEntity1).Should().BeFalse("name does not match the second filter");
    compiled(nonMatchingEntity2).Should().BeFalse("ID does not match the first filter");
  }

  [Fact]
  public void AddFilter_WithEmptyCriteria_WorksCorrectly()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var entity = new TestEntity() { Id = Guid.NewGuid(), Name = "Irrelevant" };

    // Act
    spec.AddFilter(e => true); // universal match
    var compiled = spec.Criterias!.Compile();

    // Assert
    spec.Criterias.Should().NotBeNull("filters should be initialized even with trivial criteria");
    compiled(entity).Should().BeTrue("the filter always returns true, regardless of entity content");
  }

  // Ordering tests
  [Fact]
  public void AddOrderBy_SetsOrderByExpression()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderBy(e => e.Name);

    // Assert
    spec.OrderBy.Should().NotBeNull("an ascending order expression was added");
    spec.OrderByDescending.Should().BeNull("only ascending order was specified, so descending should remain unset");
  }

  [Fact]
  public void AddOrderBy_WithNullExpression_DoesNothing()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderBy(e => e.Name);
    spec.AddOrderBy<string>(null!);

    // Assert
    spec.OrderBy.Should().NotBeNull("a valid OrderBy was previously set and should remain unchanged");
    GetPropertyNameFromExpression(spec.OrderBy).Should().Be(nameof(TestEntity.Name),
                                                            "null expression should not override existing OrderBy");
  }

  [Fact]
  public void AddOrderBy_ReplacesPreviousOrdering()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderBy(e => e.Name); // initial ordering
    spec.AddOrderBy(e => e.Age);  // replaces previous

    // Assert
    spec.OrderBy.Should().NotBeNull("a new ordering was applied and should override the previous one");
    spec.OrderByDescending.Should().BeNull("no descending ordering was specified");
    GetPropertyNameFromExpression(spec.OrderBy).Should().Be(nameof(TestEntity.Age), "the last AddOrderBy call should define the active ordering");
  }

  [Fact]
  public void AddOrderByDescending_ReplacesPreviousOrdering()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderByDescending(e => e.Name);
    spec.AddOrderByDescending(e => e.Age);

    // Assert
    spec.OrderBy.Should().BeNull("only descending ordering was applied");
    spec.OrderByDescending.Should().NotBeNull("a new descending ordering was set and should override the previous one");
    GetPropertyNameFromExpression(spec.OrderByDescending).Should().Be(nameof(TestEntity.Age),
                                                                      "the last AddOrderByDescending call should define the active ordering");
  }

  [Fact]
  public void AddOrderBy_AfterOrderByDescending_ReplacesIt()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderByDescending(e => e.Name); // initial descending ordering
    spec.AddOrderBy(e => e.Age);            // replaces with ascending ordering

    // Assert
    spec.OrderBy.Should().NotBeNull("ascending ordering was explicitly set and should override the previous descending one");
    spec.OrderByDescending.Should().BeNull("descending ordering should be cleared when ascending ordering is applied");
    GetPropertyNameFromExpression(spec.OrderBy).Should().Be(nameof(TestEntity.Age),
                                                            "the last AddOrderBy call should define the active ordering");
  }

  [Fact]
  public void AddOrderByDescending_AfterOrderBy_ReplacesIt()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddOrderBy(e => e.Name);
    spec.AddOrderByDescending(e => e.Age);

    // Assert
    spec.OrderBy.Should().BeNull("descending ordering overrides any previously set ascending order");
    spec.OrderByDescending.Should().NotBeNull("a new descending ordering was explicitly applied");
    GetPropertyNameFromExpression(spec.OrderByDescending).Should().Be(nameof(TestEntity.Age),
                                                                      "the last AddOrderByDescending call should define the active ordering");
  }

  // Including tests
  [Fact]
  public void AddInclude_WithNullExpression_ThrowsException()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    Action act = () => spec.AddInclude<ChildEntity>(null!);

    // Assert
    act.Should().Throw<ArgumentNullException>().WithMessage("Include expression cannot be null. (Parameter 'include')",
                                                            "null include expression should not be accepted");
  }

  [Fact]
  public void AddInclude_AddsToIncludeChains()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddInclude(e => e.Children);

    // Assert
    spec.IncludeChains.Should().ContainSingle("one include chain should be created");
    spec.IncludeChains[0].Includes[0].EntityType.Should().Be<TestEntity>("the include expression starts from TestEntity");
    spec.IncludeChains[0].Includes[0].PropertyType.Should().Be<List<ChildEntity>>("the included property is a collection of ChildEntity");
  }

  [Fact]
  public void AddThenInclude_WithoutInclude_ThrowsException()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    Action act = () => spec.AddThenInclude<ChildEntity, string>(c => c.Name);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("No Include found to apply ThenInclude",
                                                                "AddThenInclude requires a preceding Include expression");
  }

  [Fact]
  public void AddThenInclude_Object_AfterInclude_AddsToChain()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddInclude(e => e.Children);
    spec.AddThenInclude<ChildEntity, string>(c => c.Name);

    // Assert
    var secondInclude = spec.IncludeChains[0].Includes[1];

    spec.IncludeChains.Should().ContainSingle("only one include chain should be created after AddInclude")
        .Which.Includes.Should().HaveCount(2);
    secondInclude.EntityType.Should().Be<ChildEntity>("ThenInclude targets the element type of the Children collection");
    secondInclude.PropertyType.Should().Be<string>("ThenInclude navigates to the 'Name' property of ChildEntity");
  }

  [Fact]
  public void AddThenInclude_ListObjects_AfterInclude_AddsToChain()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddInclude(e => e.Children);
    spec.AddThenInclude<ChildEntity, List<ToyEntity>>(c => c.Toys);

    // Assert
    var secondInclude = spec.IncludeChains[0].Includes[1];

    spec.IncludeChains.Should().ContainSingle("only one include chain should be created after AddInclude")
        .Which.Includes.Should().HaveCount(2);
    secondInclude.EntityType.Should().Be<ChildEntity>("ThenInclude targets the element type of the Children collection");
    secondInclude.PropertyType.Should().Be<List<ToyEntity>>("ThenInclude navigates to the 'Toys' property of ChildEntity");
  }

  [Fact]
  public void AddThenInclude_MultipleTimes_AddsAllToChain()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    spec.AddInclude(e => e.Children);
    spec.AddThenInclude<ChildEntity, List<ToyEntity>>(c => c.Toys);

    // Assert
    spec.IncludeChains.Should().ContainSingle("only one include chain should be created");
    spec.IncludeChains[0].Includes.Should().HaveCount(2, "each ThenInclude should extend the chain by one step");

    var includes = spec.IncludeChains[0].Includes;

    includes[0].EntityType.Should().Be<TestEntity>("first include starts from TestEntity");
    includes[0].PropertyType.Should().Be<List<ChildEntity>>("Children is a collection of ChildEntity");

    includes[1].EntityType.Should().Be<ChildEntity>("second include navigates into ChildEntity");
    includes[1].PropertyType.Should().Be<List<ToyEntity>>("Toys is a collection of ToyEntity");
  }


  // Pagination test
  [Fact]
  public void ConfigurePagination_CalculatesSkipAndTake()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var context = new PaginationContext(2, 10);

    // Act
    spec.ConfigurePagination(context);

    // Assert
    spec.Skip.Should().Be(10, "page 2 with page size 10 should skip the first 10 items");
    spec.Take.Should().Be(10, "page size defines the number of items to take");
  }

  [Fact]
  public void ConfigurePagination_WithNullContext_ThrowsException()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();

    // Act
    Action act = () => spec.ConfigurePagination(null!);

    // Assert

    act.Should().Throw<ArgumentNullException>().WithMessage("PaginationContext cannot be null. (Parameter 'paginationContext')",
                                                            "null paginationContext should not be accepted");
  }

  [Theory]
  [InlineData(0, 0, 0, 10)]   // Page 0 -> treated as Page 1, default page size
  [InlineData(-1, -5, 0, 10)] // Negative values -> defaults applied
  [InlineData(1, 10, 0, 10)]  // Page 1, size 10 → skip 0
  [InlineData(2, 20, 20, 20)] // Page 2, size 20 → skip 20
  [InlineData(3, 15, 30, 15)] // Page 3, size 15 → skip 30
  public void ConfigurePagination_WithVariousValues_CalculatesCorrectly(int pageNumber, int pageSize, int expectedSkip, int expectedTake)
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var context = new PaginationContext(pageNumber, pageSize);

    // Act
    spec.ConfigurePagination(context);

    // Assert
    spec.Skip.Should().Be(expectedSkip, $"pageNumber={pageNumber}, pageSize={pageSize} → expected Skip={expectedSkip}");
    spec.Take.Should().Be(expectedTake, $"pageNumber={pageNumber}, pageSize={pageSize} → expected Take={expectedTake}");
  }

  [Fact]
  public void ConfigurePagination_WithMaxValues_HandlesCorrectly()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var context = new PaginationContext(int.MaxValue, int.MaxValue);

    // Act
    spec.ConfigurePagination(context);

    // Assert
    spec.Skip.Should().BeGreaterThanOrEqualTo(0, "Skip should never be negative, even with extreme page numbers");
    spec.Take.Should().BeGreaterThanOrEqualTo(0, "Take should be clamped or defaulted to prevent excessive load");
    spec.Take.Should().BeLessThanOrEqualTo(GenericSpecification<TestEntity>.MaxPageSize, "Take should be clamped to a reasonable max to avoid performance issues");
  }

  // Clone test
  [Fact]
  public void Clone_CopiesAllProperties()
  {
    // Arrange
    var spec = new GenericSpecification<TestEntity>();
    var id = Guid.NewGuid();

    // Act
    spec.AddFilter(e => e.Id == id);
    spec.AddInclude(e => e.Children);
    spec.AddOrderBy(e => e.Name);
    var clone = spec.Clone();

    // Assert
    spec.Criterias?.ToString().Should().Be(clone.Criterias?.ToString(),
                                           "filter expression should be cloned exactly");
    spec.OrderBy?.ToString().Should().Be(clone.OrderBy?.ToString(),
                                         "order expression should be preserved in clone");
    spec.IncludeChains.Should().HaveSameCount(clone.IncludeChains,
                                              "include chains should be copied fully");
  }

  [Fact]
  public void Clone_CreatesIndependentCopy()
  {
    // Arrange
    var original = new GenericSpecification<TestEntity>();
    var pagination = new PaginationContext(2, 10);
    original.AddFilter(e => e.Id == Guid.NewGuid());
    original.AddInclude(e => e.Children);
    original.AddOrderBy(e => e.Name);
    original.ConfigurePagination(pagination);

    // Act
    var clone = original.Clone();
    original.AddFilter(e => e.Age > 18);
    original.AddThenInclude<ChildEntity, List<ToyEntity>>(c => c.Toys);
    original.AddOrderByDescending(e => e.Age);
    
    // Assert — clone state
    clone.Criterias.Should().NotBeNull("clone should preserve original filter");
    clone.IncludeChains[0].Includes.Should().HaveCount(1, "clone should preserve original includes only");
    clone.OrderBy.Should().NotBeNull("clone should preserve original OrderBy");
    clone.OrderByDescending.Should().BeNull("clone should not inherit later OrderByDescending");
    clone.Skip.Should().Be(10, "clone should preserve pagination state");
    clone.Take.Should().Be(10);

    // Assert — original state
    original.Criterias.Should().NotBeNull("original should reflect added filter after cloning");
    original.IncludeChains[0].Includes.Should().HaveCount(2, "original should reflect added ThenInclude");
    original.OrderBy.Should().BeNull("original OrderBy should be replaced");
    original.OrderByDescending.Should().NotBeNull("original should reflect new OrderByDescending");
  }
}
