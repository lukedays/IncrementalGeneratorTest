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
                var method = (IMethodSymbol)context.TargetSymbol;
                var methodName = method.Name;

                return new MethodNode
                {
                    ClassName = className,
                    ClassAccess = containingClass.DeclaredAccessibility.ToString().ToLower(),
                    ClassIsStatic = containingClass.IsStatic ? "static" : "",
                    Namespace = ns,
                    MethodName = methodName,
                    ReturnType = method.ReturnType,
                    ParamsDefinitions = string.Join(
                        ", ",
                        method.Parameters.Select(x => x.OriginalDefinition.ToString()).ToList()
                    ),
                    ParamsNames = method.Parameters.Select(x => x.Name).ToList(),
                    MethodIsStatic = method.IsStatic ? "static" : "",
                    MethodAccess = method.DeclaredAccessibility.ToString().ToLower(),
                    MethodId = $"{ns}.{className}.{methodName}",
                    CacheExpiration = (double)context.Attributes[0].ConstructorArguments[0].Value!
                };
            }
        );

        // Add the final source for the augmented methods
        initContext.RegisterSourceOutput(
            methodNodes,
            (context, node) =>
            {
                var sourceText = SourceText.From(
                    $$"""
namespace {{node.Namespace}};
using {{generatedNs}};
using Microsoft.Extensions.Caching.Memory;

{{node.ClassAccess}} {{node.ClassIsStatic}} partial class {{node.ClassName}}
{
    {{node.MethodAccess}} {{node.MethodIsStatic}} {{node.ReturnType}} {{node.MethodName}}Cached({{node.ParamsDefinitions}})
    {
        var key = $"{{node.MethodId}}.{{string.Join(
                        ".",
                        node.ParamsNames.Select(x => $"{{{x}}}")
                    )}}";

        if ({{generatedService}}.Cache.TryGetValue(key, out {{node.ReturnType}} value))
        {
            return value;
        }

        value = {{node.MethodName}}({{string.Join(", ", node.ParamsNames)}});
        {{generatedService}}.Cache.Set(key, value, TimeSpan.FromMinutes({{node.CacheExpiration}}));
        return value;
    }
}
""",
                    Encoding.UTF8
                );

                context.AddSource($"{node.MethodId}.g.cs", sourceText);
            }
        );
    }
}
