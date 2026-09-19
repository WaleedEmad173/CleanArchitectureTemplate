namespace CleanArchitectureTemplate.Application.Common.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    PaginationMeta Meta);
