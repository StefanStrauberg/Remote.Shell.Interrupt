namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

/// <summary>
/// Represents parameters for a request, including pagination details and filters.
/// </summary>
public class RequestParameters
{
  /// <summary>
  /// The maximum allowed page size.
  /// </summary>
  const int MAX_PAGE_SIZE = 50;

  /// <summary>
  /// Stores the current page size. Default value is 10.
  /// </summary>
  int? _pageSize;

  /// <summary>
  /// Stores the current page number. Default value is 1.
  /// </summary>
  int? _pageNumber;

  /// <summary>
  /// Gets or sets the page number.
  /// </summary>
  /// <remarks>
  /// The value must be greater than 0; otherwise, an exception will be thrown.
  /// </remarks>
  /// <exception cref="ArgumentOutOfRangeException">
  /// Thrown when the value is less than 1.
  /// </exception>
  public int? PageNumber 
  { 
      get => _pageNumber; 
      set
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(value), "PageNumber must be greater than 0.");

        _pageNumber = value;
      }
  }

  /// <summary>
  /// Gets or sets the page size.
  /// </summary>
  /// <remarks>
  /// The value must be greater than 0. If the value exceeds the maximum page size, it will be capped at the maximum.
  /// </remarks>
  /// <exception cref="ArgumentOutOfRangeException">
  /// Thrown when the value is less than 1.
  /// </exception>
  public int? PageSize 
  { 
      get => _pageSize;
      set 
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(value), "PageSize must be greater than 0.");

        _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
      }
  }

  /// <summary>
  /// Gets or sets the list of filters to be applied to the request.
  /// </summary>
  public List<FilterDescriptor>? Filters { get; set; } = [];

  /// <summary>
  /// Enable pagination when size and page number are set.
  /// </summary>
  public bool IsPaginated 
    => _pageNumber is not null && _pageSize is not null;
}
