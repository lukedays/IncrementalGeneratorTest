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
        initContext.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                $"{generatedAttrib}.g.cs",
                SourceText.From(
                    $$"""
namespace {{generatedNs}};
using System;
using Microsoft.Extensions.Caching.Memory;

[AttributeUsage(AttributeTargets.Method)]
internal class {{generatedAttrib}} : Attribute
{
    internal {{generatedAttrib}}(double absoluteExpirationInMinutes = 30) { }
}

internal static class {{generatedService}}
{
    internal static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
}
""",
                    Encoding.UTF8
                )
            );
        });

        // Retrieve method nodes with the cache attribute
        var nodes = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            $"{generatedNs}.{generatedAttrib}",
            static (syntaxNode, _) => syntaxNode is BaseMethodDeclarationSyntax,
            static (context, _) => Helpers.GetMethodInfo(context, "Cache")
        );

        // Add the final source for the augmented methods
        initContext.RegisterSourceOutput(
            nodes,
            static (context, node) =>
            {
                var sourceText = SourceText.From(
                    $$"""
namespace {{node.Namespace}};
using {{generatedNs}};
using Microsoft.Extensions.Caching.Memory;

{{node.ClassModifiers}} class {{node.ClassName}}{{node.ClassTypeParameters}} {{node.ClassConstraints}}
{
    {{node.MethodModifiers}} {{node.ReturnType}} {{node.MethodName}}Cached{{node.MethodTypeParameters}}({{node.ParamsDefinitions}}) {{node.MethodConstraints}}
    {
        var key = $"{{node.CacheKey}}";

        if ({{generatedService}}.Cache.TryGetValue(key, out {{node.ReturnType}} value))
        {
            return value;
        }

        value = {{node.MethodName}}({{node.ParamsCall}});
        {{generatedService}}.Cache.Set(key, value, TimeSpan.FromMinutes({{node.CacheExpiration}}));
        return value;
    }
}
""",
                    Encoding.UTF8
                );

                context.AddSource($"{node.Filename}.g.cs", sourceText);
            }
        );
    }
}
