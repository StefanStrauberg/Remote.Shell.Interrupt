using Remote.Shell.Interrupt.Storehouse.Application.Models.Factories;

namespace Tests.Application.Models;

public class RequestParametersTests
{
    [Fact]
    public void NewInstance_IsNotPaginated()
    {
        var parameters = new RequestParameters();

        parameters.IsPaginated.Should().BeFalse();
        parameters.Filters.Should().BeEmpty();
        parameters.OrderBy.Should().BeNull();
        parameters.OrderByDescending.Should().BeFalse();
    }

    [Fact]
    public void PageNumberAndPageSizeSet_IsPaginated()
    {
        var parameters = new RequestParameters
        {
            PageNumber = 3,
            PageSize = 20
        };

        parameters.IsPaginated.Should().BeTrue();
        parameters.PageNumber.Should().Be(3);
        parameters.PageSize.Should().Be(20);
    }

    [Fact]
    public void PageSizeAboveMaximum_IsCappedAt50()
    {
        var parameters = new RequestParameters { PageSize = 1000 };

        parameters.PageSize.Should().Be(50);
    }

    [Fact]
    public void PageNumberBelowOne_ThrowsArgumentOutOfRange()
    {
        var act = () => new RequestParameters { PageNumber = 0 };

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void PageSizeBelowOne_ThrowsArgumentOutOfRange()
    {
        var act = () => new RequestParameters { PageSize = 0 };

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void PageNumberNegative_ThrowsArgumentOutOfRange()
    {
        var act = () => new RequestParameters { PageNumber = -5 };

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}

public class RequestParametersFactoryTests
{
    [Fact]
    public void ForVlanTag_CreatesEqualsFilterOnIdVlan()
    {
        var parameters = RequestParametersFactory.ForVlanTag(77);

        parameters.Filters.Should().ContainSingle().Which.Should().Match<FilterDescriptor>(f =>
            f.PropertyPath == nameof(SPRVlan.IdVlan) &&
            f.Operator == FilterOperator.Equals &&
            f.Value == "77");
    }

    [Fact]
    public void ForClientId_CreatesEqualsFilterOnIdClient()
    {
        var parameters = RequestParametersFactory.ForClientId(9);

        parameters.Filters.Should().ContainSingle().Which.Should().Match<FilterDescriptor>(f =>
            f.PropertyPath == nameof(Client.IdClient) &&
            f.Operator == FilterOperator.Equals &&
            f.Value == "9");
    }

    [Fact]
    public void ForId_CreatesEqualsFilterOnGuidId()
    {
        var id = Guid.NewGuid();

        var parameters = RequestParametersFactory.ForId(id);

        parameters.Filters.Should().ContainSingle().Which.Should().Match<FilterDescriptor>(f =>
            f.PropertyPath == "Id" &&
            f.Operator == FilterOperator.Equals &&
            f.Value == id.ToString());
    }

    [Fact]
    public void ForNetworkDevicesByVlans_CreatesInFilterOnNestedVlanTagPath()
    {
        var parameters = RequestParametersFactory.ForNetworkDevicesByVlans([100, 200]);

        parameters.Filters.Should().ContainSingle().Which.Should().Match<FilterDescriptor>(f =>
            f.PropertyPath == "PortsOfNetworkDevice.VLANs.VLANTag" &&
            f.Operator == FilterOperator.In &&
            f.Value == "100, 200");
    }

    [Fact]
    public void Empty_ReturnsParametersWithEmptyFilterList()
    {
        var parameters = RequestParametersFactory.Empty();

        parameters.Filters.Should().NotBeNull();
        parameters.Filters.Should().BeEmpty();
    }
}
