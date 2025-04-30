namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

public class RequestParameters
{
    const int MAX_PAGE_SIZE = 50;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    /// <summary>
    /// Page number. The value should be greater than 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw, when value less or equals than 0.
    /// </exception>
    public int PageNumber 
    { 
        get => _pageNumber; 
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "PageNumber must be greater than 0.");
            else
                _pageNumber = value;
        }
    }

    /// <summary>
    /// Page size. The value should be greater tgan 0 and less than max page size.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Throw, when value less or equals than 0.
    /// </exception>
    public int PageSize 
    { 
        get => _pageSize;
        set 
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "PageSize must be greater than 0.");
            
            _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
        }
    }

    public string? Filters { get; set; } // Пример: "Name==ACME"
    public string? Sorts { get; set; }   // Пример: "-Name" (DESC), "Name" (ASC)
}