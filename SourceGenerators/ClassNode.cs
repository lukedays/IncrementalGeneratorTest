namespace SourceGenerators;

public record struct ClassNode
{
    public string ClassId { get; set; }
    public List<MethodNode> Methods { get; set; }
    public string? Namespace { get; set; }
    public string ClassAccess { get; set; }
    public string ClassName { get; set; }
}
