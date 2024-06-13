namespace SourceGenerators;

[AttributeUsage(AttributeTargets.Method)]
public class GenerateDecoratedMethodAttribute(
    string decoratorName,
    string decoratedMethodAccessibility = "public"
) : Attribute
{ }
