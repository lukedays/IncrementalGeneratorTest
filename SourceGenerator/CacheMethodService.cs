using Microsoft.Extensions.Caching.Memory;

namespace SourceGenerator;

public static class CacheMethodService
{
    public static readonly MemoryCache Cache = new(new MemoryCacheOptions());
}
