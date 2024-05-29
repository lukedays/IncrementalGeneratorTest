using Microsoft.CodeAnalysis;

namespace SourceGenerators;

public record struct MethodNode
{
    public string ClassName { get; set; }
    public string ClassAccess { get; set; }
    public string? Namespace { get; set; }
    public string MethodName { get; set; }
    public ITypeSymbol ReturnType { get; set; }
    public List<string> ParamsDefinitions { get; set; }
    public List<string> ParamsNames { get; set; }
    public string IsStatic { get; set; }
    public string MethodAccess { get; set; }
    public string ClassId { get; set; }
    public double CacheExpiration { get; set; }
    public string MethodId { get; internal set; }
}
