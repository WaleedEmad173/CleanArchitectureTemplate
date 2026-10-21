namespace CleanArchitectureTemplate.Application.Common.Pagination;

public class QueryParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool IsDescending { get; set; } = false;
}
