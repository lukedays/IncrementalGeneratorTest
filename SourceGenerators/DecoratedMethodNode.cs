using Microsoft.CodeAnalysis;

namespace SourceGenerators;

public record struct DecoratedMethodNode
{
    public string ClassName { get; set; }
    public string ClassStart { get; set; }
    public string? Namespace { get; set; }
    public string MethodName { get; set; }
    public ITypeSymbol ReturnType { get; set; }
    public string ParamsDefinitions { get; set; }
    public List<string> ParamsNames { get; set; }
    public string MethodIsStatic { get; set; }
    public string MethodAccess { get; set; }
    public string DecoratorName { get; set; }
    public bool ChangeToPublic { get; set; }
    public string MethodId { get; set; }
}
