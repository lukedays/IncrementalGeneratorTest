using System.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators;

internal static class Helpers
{
    public static MethodNodeInfo ExtractMethodInfo(
        GeneratorAttributeSyntaxContext context,
        string generatorType
    )
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
        var methodSyntax = (MethodDeclarationSyntax)context.TargetNode;
        var classSyntax = (ClassDeclarationSyntax)methodSyntax.Parent!;

        var methodId = $"{ns}.{className}.{methodName}";

        var iEnumSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
            typeof(IEnumerable).FullName
        );
        var paramsPlaceholders = method.Parameters.Select(p =>
            ParamHasInterface(p, iEnumSymbol)
                ? $"{{string.Join(\",\", {p.Name}.Select(__param__ => __param__.ToString()))}}"
                : $"{{{p.Name}}}"
        );
        var cacheKey = $"{methodId}.{string.Join(".", paramsPlaceholders)}";

        return new MethodNodeInfo
        {
            ClassName = className,
            ClassModifiers = classSyntax.Modifiers.ToString(),
            ClassConstraints = classSyntax.ConstraintClauses.ToString(),
            ClassTypeParameters = classSyntax.TypeParameterList?.ToString() ?? "",
            Namespace = ns,
            ReturnType = method.ReturnType,
            ParamsDefinitions = methodSyntax.ParameterList.ToString(),
            ParamsCall = string.Join(",", method.Parameters.Select(p => p.Name)),
            MethodName = methodName,
            MethodAccessibility = method.DeclaredAccessibility.ToString().ToLower(),
            MethodModifiers = methodSyntax.Modifiers.ToString(),
            MethodConstraints = methodSyntax.ConstraintClauses.ToString(),
            MethodTypeParameters = methodSyntax.TypeParameterList?.ToString() ?? "",
            Filename = methodId,
            CacheKey = cacheKey,
            CacheExpiration =
                generatorType == "Cache"
                    ? (double)context.Attributes[0].ConstructorArguments[0].Value!
                    : null,
            DecoratorName =
                generatorType == "Decorator"
                    ? (string)context.Attributes[0].ConstructorArguments[0].Value!
                    : null,
            DecoratedMethodAccessibility =
                generatorType == "Decorator"
                    ? (string)context.Attributes[0].ConstructorArguments[1].Value!
                    : null,
        };
    }

    private static bool ParamHasInterface(
        IParameterSymbol paramSymbol,
        INamedTypeSymbol? targetInterface
    )
    {
        return paramSymbol.Type.AllInterfaces.Any(symbol =>
            SymbolEqualityComparer.Default.Equals(symbol, targetInterface)
        );
    }
}
