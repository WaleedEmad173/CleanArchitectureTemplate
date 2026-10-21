namespace CleanArchitectureTemplate.Application.Common.Pagination;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public PaginationMeta Meta { get; init; } = default!;

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> items, PaginationMeta meta)
    {
        Items = items;
        Meta = meta;
    }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
        Meta = new PaginationMeta(pageNumber, pageSize, totalCount, totalPages);
    }
}
