using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    IMemoryCache cache,
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
            return await next();
        }

        if (cache.TryGetValue(cacheableQuery.CacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation("Returning cached value for {CacheKey}", cacheableQuery.CacheKey);
            return cachedResponse!;
        }

        logger.LogInformation("Cache miss for {CacheKey}. Fetching from source.", cacheableQuery.CacheKey);
        
        var response = await next();

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = cacheableQuery.SlidingExpiration ?? TimeSpan.FromMinutes(10)
        };

        cache.Set(cacheableQuery.CacheKey, response, options);

        return response;
    }
}
