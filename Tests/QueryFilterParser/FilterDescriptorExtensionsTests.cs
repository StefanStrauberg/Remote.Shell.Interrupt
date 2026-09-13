using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

namespace Tests.QueryFilterParser;

public class FilterDescriptorExtensionsTests
{
    static Expression<Func<T, bool>> Build<T>(string path, FilterOperator op, string value)
        => new FilterDescriptor(path, op, value).ToExpression<T>();

    [Fact]
    public void ToExpression_NullFilter_ThrowsArgumentNullException()
    {
        FilterDescriptor filter = null!;

        var act = () => filter.ToExpression<Client>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToExpression_InOperator_MatchesAnyListedValue()
    {
        var func = Build<Client>(nameof(Client.IdClient), FilterOperator.In, "1,5,10").Compile();

        func(new Client { IdClient = 5 }).Should().BeTrue();
        func(new Client { IdClient = 1 }).Should().BeTrue();
        func(new Client { IdClient = 7 }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_WordOperator_MatchesWholeWordOnly()
    {
        var func = Build<Client>(nameof(Client.Name), FilterOperator.Word, "Test").Compile();

        func(new Client { Name = "My Test Client" }).Should().BeTrue();
        func(new Client { Name = "MyTestClient" }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_EnumProperty_ParsesEnumNameCaseInsensitively()
    {
        var func = Build<Gate>(nameof(Gate.TypeOfNetworkDevice), FilterOperator.Equals, "juniper").Compile();

        func(new Gate { TypeOfNetworkDevice = TypeOfNetworkDevice.Juniper }).Should().BeTrue();
        func(new Gate { TypeOfNetworkDevice = TypeOfNetworkDevice.Cisco }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_LongProperty_ConvertsDottedIpValue()
    {
        var func = Build<Gate>(nameof(Gate.IPAddress), FilterOperator.Equals, "192.168.1.1").Compile();

        func(new Gate { IPAddress = 3232235777 }).Should().BeTrue();
        func(new Gate { IPAddress = 3232235778 }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_LongProperty_ConvertsNumericValue()
    {
        var func = Build<Port>(nameof(Port.InterfaceSpeed), FilterOperator.GreaterThan, "1000000000").Compile();

        func(new Port { InterfaceSpeed = 2_000_000_000 }).Should().BeTrue();
        func(new Port { InterfaceSpeed = 500_000_000 }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_NestedCollectionPath_UsesAnySemantics()
    {
        var func = Build<Client>($"{nameof(Client.SPRVlans)}.{nameof(SPRVlan.IdVlan)}", FilterOperator.Equals, "100").Compile();

        func(new Client { SPRVlans = [new SPRVlan { IdVlan = 100 }] }).Should().BeTrue();
        func(new Client { SPRVlans = [new SPRVlan { IdVlan = 200 }] }).Should().BeFalse();
        func(new Client()).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_NestedSingleProperty_ChainsMemberAccess()
    {
        var func = Build<Client>($"{nameof(Client.COD)}.{nameof(COD.NameCOD)}", FilterOperator.Equals, "DC-1").Compile();

        func(new Client { COD = new COD { NameCOD = "DC-1" } }).Should().BeTrue();
        func(new Client { COD = new COD { NameCOD = "DC-2" } }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_StringOperatorOnNonStringProperty_ThrowsInvalidOperation()
    {
        var act = () => Build<Client>(nameof(Client.IdClient), FilterOperator.Contains, "5");

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*can only be used with string properties*");
    }

    [Fact]
    public void ToExpression_NonExistentProperty_ThrowsArgumentException()
    {
        var act = () => Build<Client>("DoesNotExist", FilterOperator.Equals, "5");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToExpression_InvalidEnumValue_ThrowsInvalidOperation()
    {
        var act = () => Build<Gate>(nameof(Gate.TypeOfNetworkDevice), FilterOperator.Equals, "NotAVendor");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToExpression_InvalidIpForLongProperty_ThrowsInvalidOperation()
    {
        var act = () => Build<Gate>(nameof(Gate.IPAddress), FilterOperator.Equals, "not-an-ip");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToExpression_NotEqualsOperator_NegatesMatch()
    {
        var func = Build<Gate>(nameof(Gate.Name), FilterOperator.NotEquals, "gw").Compile();

        func(new Gate { Name = "other" }).Should().BeTrue();
        func(new Gate { Name = "gw" }).Should().BeFalse();
    }

    [Fact]
    public void ToExpression_StartsWithAndEndsWith_CompareEdges()
    {
        var starts = Build<Client>(nameof(Client.Name), FilterOperator.StartsWith, "Alpha").Compile();
        var ends = Build<Client>(nameof(Client.Name), FilterOperator.EndsWith, "Omega").Compile();

        starts(new Client { Name = "AlphaCorp" }).Should().BeTrue();
        starts(new Client { Name = "BetaAlpha" }).Should().BeFalse();
        ends(new Client { Name = "BetaOmega" }).Should().BeTrue();
        ends(new Client { Name = "Beta" }).Should().BeFalse();
    }
}

public class CommonQueryFilterParserGroupingTests
{
    private readonly IQueryFilterParser _parser = new CommonQueryFilterParser();

    [Fact]
    public void ParseFilters_SamePropertyFilters_CombineWithOr()
    {
        var filters = new List<FilterDescriptor>
        {
            new(nameof(Client.Name), FilterOperator.Equals, "Alpha"),
            new(nameof(Client.Name), FilterOperator.Equals, "Beta")
        };

        var func = _parser.ParseFilters<Client>(filters)!.Compile();

        func(new Client { Name = "Alpha" }).Should().BeTrue();
        func(new Client { Name = "Beta" }).Should().BeTrue();
        func(new Client { Name = "Gamma" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_DifferentPropertyFilters_CombineWithAnd()
    {
        var filters = new List<FilterDescriptor>
        {
            new(nameof(Client.Name), FilterOperator.Equals, "Alpha"),
            new(nameof(Client.Working), FilterOperator.Equals, "True")
        };

        var func = _parser.ParseFilters<Client>(filters)!.Compile();

        func(new Client { Name = "Alpha", Working = true }).Should().BeTrue();
        func(new Client { Name = "Alpha", Working = false }).Should().BeFalse();
        func(new Client { Name = "Beta", Working = true }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_ThreeDistinctProperties_CombinesAllWithAnd()
    {
        var filters = new List<FilterDescriptor>
        {
            new(nameof(Client.IdClient), FilterOperator.GreaterThan, "1"),
            new(nameof(Client.Working), FilterOperator.Equals, "True"),
            new(nameof(Client.Name), FilterOperator.StartsWith, "Alpha")
        };

        var func = _parser.ParseFilters<Client>(filters)!.Compile();

        func(new Client { IdClient = 5, Working = true, Name = "AlphaCorp" }).Should().BeTrue();
        func(new Client { IdClient = 5, Working = false, Name = "AlphaCorp" }).Should().BeFalse();
        func(new Client { IdClient = 5, Working = true, Name = "BetaCorp" }).Should().BeFalse();
        func(new Client { IdClient = 0, Working = true, Name = "AlphaCorp" }).Should().BeFalse();
    }

    [Fact]
    public void ParseFilters_RangeOnSameProperty_CombinesWithOrPerGroup()
    {
        // Filters on one property are OR-ed within the group.
        var filters = new List<FilterDescriptor>
        {
            new(nameof(Client.IdClient), FilterOperator.GreaterThan, "1"),
            new(nameof(Client.IdClient), FilterOperator.LessThan, "10"),
            new(nameof(Client.Working), FilterOperator.Equals, "True")
        };

        var func = _parser.ParseFilters<Client>(filters)!.Compile();

        func(new Client { IdClient = 5, Working = true }).Should().BeTrue();
        func(new Client { IdClient = 5, Working = false }).Should().BeFalse();
        func(new Client { IdClient = 50, Working = true }).Should().BeTrue();
    }

    [Fact]
    public void ParseOrderBy_NestedPath_BuildsAccessorExpression()
    {
        var expression = _parser.ParseOrderBy<Client>($"{nameof(Client.COD)}.{nameof(COD.NameCOD)}");

        expression.Should().NotBeNull();
        var func = expression!.Compile();
        var client = new Client { COD = new COD { NameCOD = "DC-1" } };
        func(client).Should().Be("DC-1");
    }

    [Fact]
    public void ParseOrderBy_NonExistentProperty_ThrowsArgumentException()
    {
        var act = () => _parser.ParseOrderBy<Client>("DoesNotExist");

        act.Should().Throw<ArgumentException>();
    }
}
