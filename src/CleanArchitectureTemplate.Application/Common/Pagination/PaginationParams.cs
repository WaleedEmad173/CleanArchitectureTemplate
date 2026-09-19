namespace CleanArchitectureTemplate.Application.Common.Pagination;

public sealed class PaginationParams
{
    private const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}
