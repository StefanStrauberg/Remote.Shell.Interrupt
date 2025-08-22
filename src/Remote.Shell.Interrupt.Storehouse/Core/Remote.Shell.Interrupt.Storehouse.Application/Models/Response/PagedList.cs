namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Response;

/// <summary>
/// Represents a paginated list of items, extending <see cref="List{T}"/> with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items contained in the paged list.</typeparam>
public class PagedList<T> : List<T>
{
    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int CurrentPage { get; private set; }

    /// <summary>
    /// Gets the total number of pages based on the total item count and page size.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Gets the total count of items across all pages.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Determines whether a previous page exists.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Determines whether a next page exists.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
    /// </summary>
    /// <param name="items">The collection of items to include in the paged list.</param>
    /// <param name="count">The total number of items.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PagedList(IEnumerable<T> items, int count, PaginationContext paginationContext)
    {
        TotalCount = count;
        PageSize = paginationContext.PageSize;
        CurrentPage = paginationContext.PageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)paginationContext.PageSize);
        AddRange(items);
    }

    /// <summary>
    /// Creates a new instance of <see cref="PagedList{T}"/> using the provided items and pagination context.
    /// </summary>
    /// <param name="items">The items to include on the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pagination">The pagination context specifying page number and page size.</param>
    /// <returns>
    /// A populated <see cref="PagedList{T}"/> containing the specified items and pagination metadata.
    /// </returns>
    /// <remarks>
    /// This factory method encapsulates pagination logic and ensures consistent instantiation.
    /// </remarks>
    public static PagedList<T> Create(IEnumerable<T> items, int totalCount, PaginationContext pagination)
        => new(items, totalCount, pagination);

    /// <summary>
    /// Returns an empty <see cref="PagedList{T}"/> with zero items and default pagination metadata.
    /// </summary>
    /// <returns>
    /// An empty <see cref="PagedList{T}"/> with <c>CurrentPage = 0</c>, <c>PageSize = 0</c>, and <c>TotalCount = 0</c>.
    /// </returns>
    /// <remarks>
    /// Useful as a default or fallback result when no data is available.
    /// </remarks>
    public static PagedList<T> Empty()
        => new([], 0, new(0, 0));
}
