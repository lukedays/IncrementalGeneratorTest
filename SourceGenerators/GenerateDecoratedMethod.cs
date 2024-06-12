using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators;

[Generator]
public class GenerateDecoratedMethod : IIncrementalGenerator
{
    const string generatedNs = "SourceGenerators";
    const string generatedAttrib = "GenerateDecoratedMethodAttribute";

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
using System;

[AttributeUsage(AttributeTargets.Method)]
internal class {{generatedAttrib}} : Attribute
{
    internal {{generatedAttrib}}(string decoratorName, string decoratedMethodAccessibility = "public") { }
}

internal interface IDecorator
{
    public void OnEntry();

    public void OnException(Exception ex);

    public void OnExit();
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
            static (context, _) => Helpers.GetMethodInfo(context, "Decorator")
        );

        // Add the final source for the augmented methods
        initContext.RegisterSourceOutput(
            nodes,
            static (context, node) =>
            {
                var decoratedMethodName = node.MethodName.Replace("Inner", "");
                var decoratedMethodModifiers = node.MethodModifiers.Replace(
                    node.MethodAccessibility,
                    node.DecoratedMethodAccessibility
                );
                var sourceText = SourceText.From(
                    $$"""
namespace {{node.Namespace}};
using {{generatedNs}};
                    
{{node.ClassModifiers}} class {{node.ClassName}}{{node.ClassTypeParameters}} {{node.ClassConstraints}}
{
    {{decoratedMethodModifiers}} {{node.ReturnType}} {{decoratedMethodName}}{{node.MethodTypeParameters}}({{node.ParamsDefinitions}}) {{node.MethodConstraints}}
    {
        var decorator = new {{node.DecoratorName}}();
        decorator.OnEntry();
        try {
            return {{node.MethodName}}({{node.ParamsCall}});
        }
        catch (Exception ex) {
            decorator.OnException(ex);
        }
        finally {
            decorator.OnExit();
        }
        return default;
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
