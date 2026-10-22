namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ICacheableQuery
{
    string CacheKey { get; }
    string? CacheTag => null;
    TimeSpan? SlidingExpiration { get; }
    TimeSpan? AbsoluteExpirationRelativeToNow { get; }
}
