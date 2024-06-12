using Microsoft.CodeAnalysis;

namespace SourceGenerators;

public record MethodNodeInfo
{
    public string ClassName { get; set; }
    public string ClassModifiers { get; set; }
    public string? Namespace { get; set; }
    public string MethodName { get; set; }
    public ITypeSymbol ReturnType { get; set; }
    public string ParamsDefinitions { get; set; }
    public string ParamsCall { get; set; }
    public string MethodModifiers { get; set; }
    public string MethodAccessibility { get; set; }
    public string Filename { get; set; }
    public string MethodConstraints { get; set; }
    public string MethodTypeParameters { get; set; }
    public string ClassConstraints { get; set; }
    public string ClassTypeParameters { get; set; }
    public double? CacheExpiration { get; set; }
    public string? DecoratorName { get; set; }
    public string? DecoratedMethodAccessibility { get; set; }
    public string CacheKey { get; set; }
}
