using CleanArchitectureTemplate.Application.Common.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitectureTemplate.Application.Common.Pagination;

public static class QueryExtensions
{
    public static IQueryable<T> Filter<T, TValue>(
        this IQueryable<T> query,
        TValue? value,
        Expression<Func<T, bool>> predicate)
    {
        if (value is null)
            return query;

        if (value is string str && string.IsNullOrWhiteSpace(str))
            return query;

        if (value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
            return query;

        return query.Where(predicate);
    }

    public static IQueryable<T> Filter<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
    public static IQueryable<T> Search<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string?>>[] propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertySelectors == null || propertySelectors.Length == 0)
            return query;

        searchTerm = searchTerm.Trim();
        var parameter = Expression.Parameter(typeof(T), "x");
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
        var searchConstant = Expression.Constant(searchTerm, typeof(string));

        Expression? combinedExpression = null;

        foreach (var selector in propertySelectors)
        {
            var rawBody = selector.Body is UnaryExpression { NodeType: ExpressionType.Convert } unary
                ? unary.Operand
                : selector.Body;

            var body = new ParameterReplacer(selector.Parameters[0], parameter).Visit(rawBody);

            var notNullCheck = Expression.NotEqual(body, Expression.Constant(null, typeof(string)));
            var containsCall = Expression.Call(body, containsMethod, searchConstant);
            var safeCondition = Expression.AndAlso(notNullCheck, containsCall);

            combinedExpression = combinedExpression == null
                ? safeCondition
                : Expression.OrElse(combinedExpression, safeCondition);
        }

        if (combinedExpression == null)
            return query;

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return query.Where(lambda);
    }
    public static IQueryable<T> Sort<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool isDescending = false,
        string defaultProperty = "Id")
    {
        var targetProperty = string.IsNullOrWhiteSpace(sortBy) ? defaultProperty : sortBy.Trim();
        var entityType = typeof(T);
        var parameter = Expression.Parameter(entityType, "x");

        Type currentType = entityType;
        Expression currentExpression = parameter;

        var propertyParts = targetProperty.Split('.');
        foreach (var part in propertyParts)
        {
            var propInfo = currentType.GetProperty(
                part,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propInfo == null)
            {
                if (targetProperty.Equals(defaultProperty, StringComparison.OrdinalIgnoreCase))
                    return query;

                return query.Sort(defaultProperty, isDescending, defaultProperty);
            }

            currentExpression = Expression.MakeMemberAccess(currentExpression, propInfo);
            currentType = propInfo.PropertyType;
        }

        var orderByExp = Expression.Lambda(currentExpression, parameter);
        var methodName = isDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        var resultExp = Expression.Call(
            typeof(Queryable),
            methodName,
            [entityType, currentType],
            query.Expression,
            Expression.Quote(orderByExp));

        return query.Provider.CreateQuery<T>(resultExp);
    }
    public static async Task<PagedResult<TDestination>> ToPagedResultAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TSource, TDestination>> selector,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PagedResult<TDestination>(items, totalCount, pageNumber, pageSize);
    }

    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
    public static async Task<ApiResponse<IReadOnlyList<TDestination>>> ToPagedApiResponseAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TSource, TDestination>> selector,
        string message = "Success",
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await query.ToPagedResultAsync(pageNumber, pageSize, selector, cancellationToken);
        return ResponseHandler.Success(pagedResult.Items, message, pagedResult.Meta);
    }

    public static async Task<ApiResponse<IReadOnlyList<T>>> ToPagedApiResponseAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        string message = "Success",
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await query.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        return ResponseHandler.Success(pagedResult.Items, message, pagedResult.Meta);
    }

    private sealed class ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == oldParam ? newParam : base.VisitParameter(node);
    }
}
