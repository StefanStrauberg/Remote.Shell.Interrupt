namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

/// <summary>
/// Represents parameters for a request, including pagination details and filters.
/// </summary>
public class RequestParameters
{
  /// <summary>
  /// The maximum allowed page size.
  /// </summary>
  const int MaxPageSize = 50;

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
    set => _pageNumber = ValidatePageNumber(value);
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
    set => _pageSize = ValidatePageSize(value);
  }

  /// <summary>
  /// Gets or sets the list of filters to be applied to the request.
  /// </summary>
  public List<FilterDescriptor>? Filters { get; set; } = [];

  /// <summary>
  /// Gets or sets the property name used for sorting.
  /// </summary>
  /// <remarks>
  /// If null or empty, no sorting will be applied.
  /// </remarks>
  public string? OrderBy { get; set; }

  /// <summary>
  /// Indicates whether sorting should be in descending order.
  /// </summary>
  /// <remarks>
  /// Applies only if <c>OrderBy</c> is set.
  /// </remarks>
  public bool OrderByDescending { get; set; } = false;

  /// <summary>
  /// Enable pagination when size and page number are set.
  /// </summary>
  public bool IsPaginated
    => _pageNumber.HasValue && _pageSize.HasValue;

  /// <summary>
  /// Validates and normalizes the page number.
  /// </summary>
  /// <param name="value">The page number to validate.</param>
  /// <returns>Normalized page number, defaulting to 1 if null.</returns>
  static int? ValidatePageNumber(int? value)
  {
    if (value < 1)
      throw new ArgumentOutOfRangeException(nameof(value), "PageNumber must be greater than 0.");

    return value ?? 1;
  }

  /// <summary>
  /// Validates and normalizes the page size.
  /// </summary>
  /// <param name="value">The page size to validate.</param>
  /// <returns>Capped page size, defaulting to <c>MaxPageSize</c> if null.</returns>
  static int? ValidatePageSize(int? value)
  {
    if (value < 1)
      throw new ArgumentOutOfRangeException(nameof(value), "PageSize must be greater than 0.");

    return Math.Min(value ?? MaxPageSize, MaxPageSize);
  }
}
