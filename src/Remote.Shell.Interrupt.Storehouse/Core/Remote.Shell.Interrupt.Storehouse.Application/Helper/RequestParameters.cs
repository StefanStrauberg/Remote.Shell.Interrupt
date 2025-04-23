namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public class RequestParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize 
    { 
        get => _pageSize;
        set 
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }

    public string? Filters { get; set; } // Пример: "Name==ACME"
    public string? Sorts { get; set; }   // Пример: "-Name" (DESC), "Name" (ASC)
}

public class FilterDescriptor
{
    public string PropertyPath { get; set; } = string.Empty; // Например: "SPRVlans.IdVlan"
    public FilterOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
}

public enum FilterOperator
{
    Equals,
    NotEquals,
    GraterThan,
    LessThan,
    Contains
}

public class UpdatedRequestParameters
{
    public List<FilterDescriptor> Filters { get; set; } = [];
    public bool IncludeRelations { get; set; }

    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize 
    { 
        get => _pageSize;
        set 
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}