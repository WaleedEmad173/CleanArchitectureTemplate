namespace CleanArchitectureTemplate.Application.Common.Pagination;

public class PaginationParams
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 10;
    public const int DefaultPageNumber = 1;

    private int _pageNumber = DefaultPageNumber;
    private int _pageSize = DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
