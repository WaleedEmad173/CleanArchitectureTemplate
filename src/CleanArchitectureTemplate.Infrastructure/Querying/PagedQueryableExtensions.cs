using CleanArchitectureTemplate.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanArchitectureTemplate.Infrastructure.Querying;

public static class PagedQueryableExtensions
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TDto>> map,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(map);

        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, PaginationParams.MaxPageSize);

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(pageNumber - 1) * pageSize;

        if (totalCount == 0 || skip >= totalCount || skip > int.MaxValue)
            return PagedResult<TDto>.Create([], totalCount, pageNumber, pageSize);

        var items = await query
            .Skip((int)skip)
            .Take(pageSize)
            .Select(map)
            .ToListAsync(cancellationToken);

        return PagedResult<TDto>.Create(items, totalCount, pageNumber, pageSize);
    }
}
