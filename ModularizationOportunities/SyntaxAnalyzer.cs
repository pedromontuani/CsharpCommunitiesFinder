namespace ModularizationOportunities;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

public class SyntaxAnalyzer
{
    private readonly SemanticModel _semanticModel;

    public SyntaxAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public IEnumerable<INamedTypeSymbol> GetExplicitlyReferencedClasses(MethodDeclarationSyntax methodDeclaration)
    {
        var referencedClasses = new HashSet<INamedTypeSymbol>();

        var nodes = methodDeclaration.DescendantNodes();
        foreach (var node in nodes)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node);
            if (symbolInfo.Symbol is ITypeSymbol typeSymbol && typeSymbol.TypeKind == TypeKind.Class)
            {
                referencedClasses.Add((INamedTypeSymbol)typeSymbol);
            }
        }

        return referencedClasses;
    }
    
    public IEnumerable<IMethodSymbol> GetCalledMethods(MethodDeclarationSyntax methodDeclaration)
    {
        var calledMethods = new List<IMethodSymbol>();

        var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocationExpressions)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
            {
                calledMethods.Add(methodSymbol);
            }
        }

        return calledMethods;
    }
    
    public static bool IsEntity(ClassDeclarationSyntax classDeclaration)
    {
        // Check for [Entity] attribute
        var hasEntityAttribute = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => a.Name.ToString() == "Entity");

        if (hasEntityAttribute)
        {
            return true;
        }

        // Check for inheritance from EntityBase
        var baseType = classDeclaration.BaseList?.Types
            .Select(bt => bt.Type.ToString())
            .FirstOrDefault();

        if (baseType != null && baseType == "EntityBase")
        {
            return true;
        }

        return false;
    }
}