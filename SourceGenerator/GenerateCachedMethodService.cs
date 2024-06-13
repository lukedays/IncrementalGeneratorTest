using Microsoft.Extensions.Caching.Memory;

namespace SourceGenerator;

public static class GenerateCachedMethodService
{
    public static readonly MemoryCache Cache = new(new MemoryCacheOptions());
}
