namespace Remote.Shell.Interrupt.Storehouse.API.Entities;

/// <summary>
/// Represents pagination information for a paged result set.
/// Commonly used to expose metadata in API responses and headers.
/// </summary>
public record PaginationMetadata
{
  /// <summary>
  /// Total number of items available across all pages.
  /// </summary>
  public int TotalCount { get; init; }

  /// <summary>
  /// Number of items per page.
  /// </summary>
  public int PageSize { get; init; }

  /// <summary>
  /// Index of the current page (1-based).
  /// </summary>
  public int CurrentPage { get; init; }

  /// <summary>
  /// Total number of pages available.
  /// </summary>
  public int TotalPages { get; init; }

  /// <summary>
  /// Indicates whether a next page exists.
  /// </summary>
  public bool HasNext { get; init; }

  /// <summary>
  /// Indicates whether a previous page exists.
  /// </summary>
  public bool HasPrevious { get; init; }
}
