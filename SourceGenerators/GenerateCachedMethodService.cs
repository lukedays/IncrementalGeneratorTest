using Microsoft.Extensions.Caching.Memory;

namespace SourceGenerators;

public static class GenerateCachedMethodService
{
    public static readonly MemoryCache Cache = new(new MemoryCacheOptions());
}
