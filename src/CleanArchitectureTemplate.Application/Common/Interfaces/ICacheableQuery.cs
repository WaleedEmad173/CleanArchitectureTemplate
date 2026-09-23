namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? SlidingExpiration { get; }
}
