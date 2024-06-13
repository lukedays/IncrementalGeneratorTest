using Microsoft.CodeAnalysis;

namespace SourceGenerator.Core;

public record MethodNodeInfo
{
    public string ClassName { get; set; } = null!;
    public string ClassModifiers { get; set; } = null!;
    public string? Namespace { get; set; }
    public string MethodName { get; set; } = null!;
    public ITypeSymbol ReturnType { get; set; } = null!;
    public string ParamsDefinitions { get; set; } = null!;
    public string ParamsCall { get; set; } = null!;
    public string MethodModifiers { get; set; } = null!;
    public string MethodAccessibility { get; set; } = null!;
    public string Filename { get; set; } = null!;
    public string MethodConstraints { get; set; } = null!;
    public string MethodTypeParameters { get; set; } = null!;
    public string ClassConstraints { get; set; } = null!;
    public string ClassTypeParameters { get; set; } = null!;
    public double? CacheExpiration { get; set; }
    public string? DecoratorName { get; set; }
    public string? DecoratedMethodAccessibility { get; set; }
    public string? CacheKey { get; set; }
}
