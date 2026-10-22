using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitectureTemplate.Application.Common.Querying;

public static class QueryableExtensions
{
    public static IQueryable<T> Filter<T>(
        this IQueryable<T> query,
        bool? enabled,
        Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(predicate);

        return enabled == true ? query.Where(predicate) : query;
    }

    public static IQueryable<T> Filter<T, TValue>(
        this IQueryable<T> query,
        TValue? value,
        Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(predicate);

        if (value is null)
            return query;

        if (value is string text && string.IsNullOrWhiteSpace(text))
            return query;

        return query.Where(predicate);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(predicate);

        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> Search<T>(
        this IQueryable<T> query,
        string? term,
        params string[] properties)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(term) || properties is null || properties.Length == 0)
            return query;

        var text = term.Trim();
        if (text.Length > QueryParams.MaxSearchLength)
            text = text[..QueryParams.MaxSearchLength].Trim();

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (words.Length == 0)
            return query;

        var x = Expression.Parameter(typeof(T), "x");
        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
        var validProperties = new List<PropertyInfo>();
        var seenProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in properties)
        {
            if (string.IsNullOrWhiteSpace(property))
                continue;

            if (!seenProperties.Add(property.Trim()))
                continue;

            var propertyInfo = typeof(T).GetProperty(
                property.Trim(),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propertyInfo is not null && propertyInfo.CanRead && propertyInfo.PropertyType == typeof(string))
            {
                validProperties.Add(propertyInfo);
            }
        }

        if (validProperties.Count == 0)
            return query;

        Expression? overallExpression = null;

        foreach (var word in words)
        {
            var capturedWord = word;
            Expression<Func<string>> wordValue = () => capturedWord;

            Expression? wordMatchExpression = null;

            foreach (var propertyInfo in validProperties)
            {
                var member = Expression.Property(x, propertyInfo);
                var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                var containsCall = Expression.Call(member, contains, wordValue.Body);
                var match = Expression.AndAlso(notNull, containsCall);

                wordMatchExpression = wordMatchExpression is null
                    ? match
                    : Expression.OrElse(wordMatchExpression, match);
            }

            if (wordMatchExpression is not null)
            {
                overallExpression = overallExpression is null
                    ? wordMatchExpression
                    : Expression.AndAlso(overallExpression, wordMatchExpression);
            }
        }

        if (overallExpression is null)
            return query;

        return query.Where(Expression.Lambda<Func<T, bool>>(overallExpression, x));
    }

    public static IQueryable<T> Sort<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool descending = false,
        params string[] allowedProperties)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return ApplyDefaultSort(query, descending, allowedProperties);
        }

        var sortClauses = sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedQueryable<T>? ordered = null;
        PropertyInfo? lastColumn = null;

        foreach (var clause in sortClauses)
        {
            var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var propertyName = parts[0];
            var isDescending = parts.Length > 1
                ? parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("descending", StringComparison.OrdinalIgnoreCase)
                : descending;

            var column = FindColumn<T>(propertyName, allowedProperties);
            if (column is null)
                continue;

            ordered = ordered is null
                ? ApplyOrderBy(query, column, isDescending, isThenBy: false)
                : ApplyOrderBy(ordered, column, isDescending, isThenBy: true);

            lastColumn = column;
        }

        if (ordered is null)
        {
            return ApplyDefaultSort(query, descending, allowedProperties);
        }

        var idProperty = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (idProperty is not null && idProperty.CanRead && lastColumn != idProperty)
        {
            ordered = ApplyOrderBy(ordered, idProperty, descending: false, isThenBy: true);
        }

        return ordered;
    }

    private static IQueryable<T> ApplyDefaultSort<T>(
        IQueryable<T> query,
        bool descending,
        IReadOnlyCollection<string>? allowedProperties)
    {
        var idProperty = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        var defaultColumn = (idProperty is not null && idProperty.CanRead)
            ? idProperty
            : (allowedProperties is not null && allowedProperties.Count > 0 ? FindColumn<T>(allowedProperties.First(), allowedProperties) : null);

        if (defaultColumn is null)
            return query;

        return ApplyOrderBy(query, defaultColumn, descending, isThenBy: false);
    }

    private static IOrderedQueryable<T> ApplyOrderBy<T>(
        IQueryable<T> query,
        PropertyInfo property,
        bool descending,
        bool isThenBy = false)
    {
        var x = Expression.Parameter(typeof(T), "x");
        var keySelector = Expression.Lambda(Expression.Property(x, property), x);

        string methodName;
        if (!isThenBy)
        {
            methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        }
        else
        {
            methodName = descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);
        }

        var call = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(keySelector));

        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(call);
    }

    private static PropertyInfo? FindColumn<T>(string? name, IReadOnlyCollection<string>? allowedProperties)
    {
        if (string.IsNullOrWhiteSpace(name) || allowedProperties is null || allowedProperties.Count == 0)
            return null;

        var allowedName = allowedProperties.FirstOrDefault(
            propertyName => string.Equals(propertyName, name.Trim(), StringComparison.OrdinalIgnoreCase));

        if (allowedName is null)
            return null;

        var property = typeof(T).GetProperty(
            allowedName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        var isColumn = property is { CanRead: true } &&
                       (property.PropertyType.IsValueType || property.PropertyType == typeof(string));

        return isColumn ? property : null;
    }
}
