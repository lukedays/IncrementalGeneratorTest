using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator.Core;

internal static class Helpers
{
    public static MethodNodeInfo GetMethodInfo(
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

        // If the type is a collection (i.e. List<string>) we need to iterate to create a unique cache key. If not, the value suffices
        var iCollSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(
            typeof(ICollection).FullName
        );
        var paramsPlaceholders = method.Parameters.Select(p =>
            ParamHasInterface(p, iCollSymbol)
                ? $"{{string.Join(\",\", {p.Name}?.Select(__param__ => __param__.ToString()) ?? [])}}"
                : $"{{{p.Name}}}"
        );
        var cacheKey = $"{methodId}.{string.Join(".", paramsPlaceholders)}";

        // Find full parameter names, with namespace and default values
        var paramsDefinitions = string.Join(
            ", ",
            method.Parameters.Select(p =>
                p.ToDisplayString(
                    SymbolDisplayFormat
                        .FullyQualifiedFormat.WithGlobalNamespaceStyle(
                            SymbolDisplayGlobalNamespaceStyle.Omitted
                        )
                        .WithParameterOptions(
                            SymbolDisplayParameterOptions.IncludeDefaultValue
                                | SymbolDisplayParameterOptions.IncludeType
                                | SymbolDisplayParameterOptions.IncludeName
                        )
                )
            )
        );

        return new MethodNodeInfo
        {
            ClassName = className,
            ClassModifiers = classSyntax.Modifiers.ToString(),
            ClassConstraints = GetTypeConstraints(containingClass.TypeParameters),
            ClassTypeParameters = classSyntax.TypeParameterList?.ToString() ?? "",
            Namespace = ns,
            ReturnType = method.ReturnType,
            ParamsDefinitions = paramsDefinitions,
            ParamsCall = string.Join(", ", method.Parameters.Select(p => p.Name)),
            MethodName = methodName,
            MethodAccessibility = method.DeclaredAccessibility.ToString().ToLower(),
            MethodModifiers = methodSyntax.Modifiers.ToString(),
            MethodConstraints = GetTypeConstraints(method.TypeParameters),
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

    private static string GetTypeConstraints(ImmutableArray<ITypeParameterSymbol> typeParams)
    {
        return string.Join(
            " ",
            typeParams.Select(t =>
            {
                var constraints = t.ConstraintTypes.Select(t => t.ToString()).ToList();
                if (t.HasReferenceTypeConstraint)
                    constraints.Add("class");
                if (t.HasConstructorConstraint)
                    constraints.Add("new()");
                if (t.HasValueTypeConstraint)
                    constraints.Add("struct");

                return $"where {t.Name} : {string.Join(",", constraints)}";
            })
        );
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
