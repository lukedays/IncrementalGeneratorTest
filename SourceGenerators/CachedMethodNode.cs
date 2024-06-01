using Microsoft.CodeAnalysis;

namespace SourceGenerators;

public record struct CachedMethodNode
{
    public string ClassName { get; set; }
    public string ClassStart { get; set; }
    public string? Namespace { get; set; }
    public string MethodName { get; set; }
    public ITypeSymbol ReturnType { get; set; }
    public string ParamsDefinitions { get; set; }
    public List<string> ParamsNames { get; set; }
    public string MethodStart { get; set; }
    public double CacheExpiration { get; set; }
    public string MethodId { get; set; }
}
