namespace SourceGenerators;

[AttributeUsage(AttributeTargets.Method)]
public class GenerateCachedMethodAttribute(double absoluteExpirationInMinutes = 30) : Attribute { }
