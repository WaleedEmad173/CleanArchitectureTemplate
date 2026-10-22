using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    IMemoryCache cache,
    ICacheVersionStore cacheVersionStore,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next(cancellationToken);
        }

        var cacheKey = cacheableQuery.CacheTag is { } tag
            ? $"{tag}:v{cacheVersionStore.GetVersion(tag)}:{cacheableQuery.CacheKey}"
            : cacheableQuery.CacheKey;

        if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation("Returning cached value for {CacheKey}", cacheKey);
            return cachedResponse!;
        }

        logger.LogInformation("Cache miss for {CacheKey}. Fetching from source.", cacheKey);

        var response = await next(cancellationToken);

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = cacheableQuery.SlidingExpiration ?? TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = cacheableQuery.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(15)
        };

        cache.Set(cacheKey, response, options);

        return response;
    }
}
