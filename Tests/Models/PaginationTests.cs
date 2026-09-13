namespace Tests.Models;

public class PaginationContextTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var ctx = new PaginationContext(5, 20);
        ctx.PageNumber.Should().Be(5);
        ctx.PageSize.Should().Be(20);
    }

    [Fact]
    public void Constructor_ZeroValues_AreAllowed()
    {
        var ctx = new PaginationContext(0, 0);
        ctx.PageNumber.Should().Be(0);
        ctx.PageSize.Should().Be(0);
    }
}

public class PagedListTests
{
    [Fact]
    public void Create_EmptyList_ReturnsEmptyPagedList()
    {
        var list = new List<string>();
        var ctx = new PaginationContext(1, 10);
        var result = PagedList<string>.Create(list, 0, ctx);

        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.HasNext.Should().BeFalse();
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void Create_FewerItemsThanPageSize_ReturnsAllItems()
    {
        var list = new[] { "a", "b", "c" }.ToList();
        var ctx = new PaginationContext(1, 10);
        var result = PagedList<string>.Create(list, 3, ctx);

        result.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
        result.HasNext.Should().BeFalse();
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void Create_MoreItemsThanPageSize_FirstPage()
    {
        var list = Enumerable.Range(1, 10).Select(i => $"item{i}").ToList();
        var ctx = new PaginationContext(1, 10);
        var result = PagedList<string>.Create(list, 25, ctx);

        result.Should().HaveCount(10);
        result[0].Should().Be("item1");
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.HasNext.Should().BeTrue();
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void Create_SecondPage_HasCorrectMetadata()
    {
        var list = Enumerable.Range(1, 10).Select(i => $"item{i}").ToList();
        var ctx = new PaginationContext(2, 10);
        var result = PagedList<string>.Create(list, 25, ctx);

        result.Should().HaveCount(10);
        result.CurrentPage.Should().Be(2);
        result.HasNext.Should().BeTrue();
        result.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void Create_LastPage_HasCorrectMetadata()
    {
        var list = Enumerable.Range(1, 5).Select(i => $"item{i}").ToList();
        var ctx = new PaginationContext(3, 10);
        var result = PagedList<string>.Create(list, 25, ctx);

        result.Should().HaveCount(5);
        result.CurrentPage.Should().Be(3);
        result.HasNext.Should().BeFalse();
        result.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void Empty_ReturnsEmptyPagedListWithZeroMetadata()
    {
        var result = PagedList<string>.Empty();
        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.CurrentPage.Should().Be(0);
        result.PageSize.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
