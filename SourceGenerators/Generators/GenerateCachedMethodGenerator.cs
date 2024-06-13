﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SourceGenerators.Generators;

[Generator]
public class GenerateCachedMethodGenerator : IIncrementalGenerator
{
    const string generatedNs = nameof(SourceGenerators);
    const string generatedAttrib = nameof(GenerateCachedMethodAttribute);
    const string generatedService = nameof(GenerateCachedMethodService);

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
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
// <autogenerated />
namespace {{node.Namespace}};
using {{generatedNs}};
using System;
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