namespace ModularizationOportunities;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

public class MethodAnalyzer
{
    private readonly SemanticModel _semanticModel;

    public MethodAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public IEnumerable<INamedTypeSymbol> GetReferencedClasses(MethodDeclarationSyntax methodDeclaration)
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
    
    public bool IsEntity(ClassDeclarationSyntax classDeclaration)
    {
        // Check for [Entity] attribute
        var hasEntityAttribute = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => _semanticModel.GetTypeInfo(a).Type?.Name == "Entity");

        if (hasEntityAttribute)
        {
            return true;
        }

        // Check for inheritance from EntityBase
        var baseType = classDeclaration.BaseList?.Types
            .Select(bt => _semanticModel.GetTypeInfo(bt.Type).Type)
            .FirstOrDefault();

        if (baseType != null && baseType.Name == "EntityBase")
        {
            return true;
        }

        return false;
    }
}