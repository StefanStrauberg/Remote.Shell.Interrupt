namespace Tests.QueryFilterParser;

public class CommonQueryFilterParserTests
{
    private readonly IQueryFilterParser _parser = new CommonQueryFilterParser();

    [Fact]
    public void ParseFilters_EmptyFilterList_ReturnsNullExpression()
    {
        var filters = new List<FilterDescriptor>();
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFilters_NullFilterList_ReturnsNullExpression()
    {
        var result = _parser.ParseFilters<Client>(null!);
        result.Should().BeNull();
    }

    [Fact]
    public void ParseFilters_SingleEqualsFilter_ReturnsExpression()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.Equals, Value = "TestClient" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        var matchingClient = new Client { Name = "TestClient" };
        var nonMatchingClient = new Client { Name = "OtherClient" };
        func(matchingClient).Should().BeTrue();
        func(nonMatchingClient).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_NotEqualsFilter_ReturnsExpression()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.NotEquals, Value = "TestClient" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { Name = "OtherClient" }).Should().BeTrue();
        func(new Client { Name = "TestClient" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_ContainsFilter_ForStringProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.Contains, Value = "Test" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { Name = "MyTestClient" }).Should().BeTrue();
        func(new Client { Name = "OtherClient" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_StartsWithFilter_ForStringProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.StartsWith, Value = "Test" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { Name = "TestClient" }).Should().BeTrue();
        func(new Client { Name = "ClientTest" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_EndsWithFilter_ForStringProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.EndsWith, Value = "Client" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { Name = "TestClient" }).Should().BeTrue();
        func(new Client { Name = "ClientTest" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_GreaterThanFilter_ForNumericProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.IdClient), Operator = FilterOperator.GreaterThan, Value = "5" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { IdClient = 10 }).Should().BeTrue();
        func(new Client { IdClient = 3 }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_LessThanFilter_ForNumericProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.IdClient), Operator = FilterOperator.LessThan, Value = "5" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { IdClient = 3 }).Should().BeTrue();
        func(new Client { IdClient = 10 }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_InFilter_ForIntProperty()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.IdClient), Operator = FilterOperator.In, Value = "1,5,10" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { IdClient = 5 }).Should().BeTrue();
        func(new Client { IdClient = 7 }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_MultipleFilters_CombinesWithAndAlso()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.Equals, Value = "TestClient" },
            new() { PropertyPath = nameof(Client.Working), Operator = FilterOperator.Equals, Value = "True" }
        };
        var result = _parser.ParseFilters<Client>(filters);
        result.Should().NotBeNull();

        var func = result!.Compile();
        func(new Client { Name = "TestClient", Working = true }).Should().BeTrue();
        func(new Client { Name = "TestClient", Working = false }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_InvalidPropertyType_ThrowsInvalidOperationException()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = nameof(Client.Name), Operator = FilterOperator.GreaterThan, Value = "5" }
        };
        Action act = () => _parser.ParseFilters<Client>(filters);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ParseFilters_NonExistentPropertyPath_ThrowsArgumentException()
    {
        var filters = new List<FilterDescriptor>
        {
            new() { PropertyPath = "NonExistentProperty", Operator = FilterOperator.Equals, Value = "5" }
        };
        Action act = () => _parser.ParseFilters<Client>(filters);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseOrderBy_ValidPropertyName_ReturnsExpression()
    {
        var result = _parser.ParseOrderBy<Client>(nameof(Client.Name));
        result.Should().NotBeNull();
    }

    [Fact]
    public void ParseOrderBy_NullPropertyName_ReturnsNull()
    {
        var result = _parser.ParseOrderBy<Client>(null);
        result.Should().BeNull();
    }

    [Fact]
    public void ParseOrderBy_WhitespacePropertyName_ReturnsNull()
    {
        var result = _parser.ParseOrderBy<Client>("   ");
        result.Should().BeNull();
    }
}
