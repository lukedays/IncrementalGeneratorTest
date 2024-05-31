using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators;

[Generator]
public class GenerateCachedMethod : IIncrementalGenerator
{
    const string generatedNs = "SourceGenerators";
    const string generatedAttrib = "GenerateCachedMethodAttribute";
    const string generatedService = "GenerateCachedMethodService";

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        // Add the source for the cache attribute
        initContext.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                $"{generatedAttrib}.g.cs",
                SourceText.From(
                    $$"""
namespace {{generatedNs}};

[AttributeUsage(AttributeTargets.Method)]
public class {{generatedAttrib}} : Attribute
{
    public {{generatedAttrib}}(double absoluteExpirationInMinutes = 30) { }
}
""",
                    Encoding.UTF8
                )
            );

            context.AddSource(
                $"{generatedService}.g.cs",
                SourceText.From(
                    $$"""
namespace {{generatedNs}};
using Microsoft.Extensions.Caching.Memory;

public static class {{generatedService}}
{
    public static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
}
""",
                    Encoding.UTF8
                )
            );
        });

        // Retrieve method nodes with the cache attribute
        var methodNodes = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            $"{generatedNs}.{generatedAttrib}",
            static (syntaxNode, _) => syntaxNode is BaseMethodDeclarationSyntax,
            static (context, _) =>
            {
                // Get class/method info
                var containingClass = context.TargetSymbol.ContainingType;
                var className = containingClass.Name;
                var ns = containingClass.ContainingNamespace?.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                        SymbolDisplayGlobalNamespaceStyle.Omitted
                    )
                );
                var classId = $"{ns}.{className}";
                var method = (IMethodSymbol)context.TargetSymbol;
                var methodName = method.Name;
                var methodId = $"{classId}.{methodName}";

                return new MethodNode
                {
                    ClassName = className,
                    ClassAccess = containingClass.DeclaredAccessibility.ToString().ToLower(),
                    Namespace = ns,
                    MethodName = methodName,
                    ReturnType = method.ReturnType,
                    ParamsDefinitions = method
                        .Parameters.Select(x => x.OriginalDefinition.ToString())
                        .ToList(),
                    ParamsNames = method.Parameters.Select(x => x.Name).ToList(),
                    IsStatic = method.IsStatic ? "static " : "",
                    MethodAccess = method.DeclaredAccessibility.ToString().ToLower(),
                    ClassId = classId,
                    MethodId = methodId,
                    CacheExpiration = (double)context.Attributes[0].ConstructorArguments[0].Value!
                };
            }
        );

        // Group method nodes by class and build class nodes
        var classNodes = methodNodes
            .Collect()
            .SelectMany(
                static (sm, _) =>
                    sm.GroupBy(g => g.ClassId)
                        .Select(g => new ClassNode
                        {
                            ClassId = g.Key,
                            Namespace = g.First().Namespace,
                            ClassAccess = g.First().ClassAccess,
                            ClassName = g.First().ClassName,
                            Methods = [.. g]
                        })
            );

        // Add the final source for the augmented methods
        initContext.RegisterSourceOutput(
            classNodes,
            (context, node) =>
            {
                // Build method texts
                var methodsText = new StringBuilder();
                foreach (var m in node.Methods)
                {
                    methodsText.AppendLine(
                        $$"""
    
{{m.MethodAccess}} {{m.IsStatic}}{{m.ReturnType}} {{m.MethodName}}Cached({{string.Join(
                    ", ",
                    m.ParamsDefinitions
                )}})
    {
        var key = $"{{m.MethodId}}.{{string.Join(".", m.ParamsNames.Select(x => $"{{{x}}}"))}}";

        if ({{generatedService}}.Cache.TryGetValue(key, out {{m.ReturnType}} value))
        {
            return value;
        }

        value = {{m.MethodName}}({{string.Join(", ", m.ParamsNames)}});

        {{generatedService}}.Cache.Set(key, value, TimeSpan.FromMinutes({{m.CacheExpiration}}));
        return value;
    }
"""
                    );
                }

                // Build class texts
                var sourceText = SourceText.From(
                    $$"""
namespace {{node.Namespace}};
using {{generatedNs}};
using Microsoft.Extensions.Caching.Memory;

{{node.ClassAccess}} partial class {{node.ClassName}}
{
{{methodsText}}
}
""",
                    Encoding.UTF8
                );

                context.AddSource($"{node.ClassId}.g.cs", sourceText);
            }
        );
    }
}
