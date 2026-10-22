using System.Collections.Concurrent;
using CleanArchitectureTemplate.Application.Common.Interfaces;

namespace CleanArchitectureTemplate.Application.Common.Services;

public sealed class CacheVersionStore : ICacheVersionStore
{
    private readonly ConcurrentDictionary<string, long> versions = new(StringComparer.Ordinal);

    public long GetVersion(string tag) => versions.GetOrAdd(tag, 0);

    public void Invalidate(string tag) => versions.AddOrUpdate(tag, 1, static (_, current) => current + 1);
}
