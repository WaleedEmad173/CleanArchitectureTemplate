namespace CleanArchitectureTemplate.Application.Common.Pagination;

public sealed record PaginationMeta(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages)
{
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
