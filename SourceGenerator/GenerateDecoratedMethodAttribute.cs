#pragma warning disable CS9113 // Parameter is unread.

namespace SourceGenerator;

[AttributeUsage(AttributeTargets.Method)]
public class GenerateDecoratedMethodAttribute(
    string decoratorName,
    string decoratedMethodAccessibility = "public"
) : Attribute { }
