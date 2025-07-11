namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Response;

/// <summary>
/// Represents the input context for defining pagination behavior in queries.
/// </summary>
/// <param name="PageNumber">The 1-based index of the page to retrieve.</param>
/// <param name="PageSize">The number of items to include per page.</param>
public record PaginationContext(int PageNumber, int PageSize);
