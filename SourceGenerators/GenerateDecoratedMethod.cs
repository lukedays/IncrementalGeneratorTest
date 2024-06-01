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

[AttributeUsage(AttributeTargets.Method)]
public class {{generatedAttrib}} : Attribute
{
    public {{generatedAttrib}}(string decoratorName, bool changeToPublic = true) { }
}

public interface IDecorator
{
    public void OnStart();

    public void OnException(Exception ex);

    public void OnEnd();
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

                return new DecoratedMethodNode
                {
                    ClassName = className,
                    ClassStart =
                        $"{containingClass.DeclaredAccessibility.ToString().ToLower()} {(containingClass.IsStatic ? "static" : "")}",
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
                    DecoratorName = (string)context.Attributes[0].ConstructorArguments[0].Value!,
                    ChangeToPublic = (bool)context.Attributes[0].ConstructorArguments[1].Value!
                };
            }
        );

        //        // Add the final source for the augmented methods
        initContext.RegisterSourceOutput(
            methodNodes,
            (context, node) =>
            {
                var access = node.ChangeToPublic ? "public" : node.MethodAccess;
                var finalMethodName = node.MethodName.Replace("Inner", "");
                var sourceText = SourceText.From(
                    $$"""
namespace {{node.Namespace}};
using {{generatedNs}};
                    
{{node.ClassStart}} partial class {{node.ClassName}}
{
    {{access}} {{node.MethodIsStatic}} {{node.ReturnType}} {{finalMethodName}}({{node.ParamsDefinitions}})
    {
        var decorator = new {{node.DecoratorName}}();
        decorator.OnStart();
        try {
            return {{node.MethodName}}({{string.Join(", ", node.ParamsNames)}});
        }
        catch (Exception ex) {
            decorator.OnException(ex);
        }
        finally {
            decorator.OnEnd();
        }
        return default;
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
