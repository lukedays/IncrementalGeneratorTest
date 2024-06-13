#pragma warning disable CS9113 // Parameter is unread.

namespace SourceGenerator;

[AttributeUsage(AttributeTargets.Method)]
public class GenerateCachedMethodAttribute(double absoluteExpirationInMinutes = 30) : Attribute { }
