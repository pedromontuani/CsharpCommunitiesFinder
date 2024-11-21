using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModularizationOportunities.utils;

public static class ClassDeclarationSyntaxHelper
{
    public static ClassDeclarationSyntax? GetClassDeclarationSyntax(INamedTypeSymbol namedTypeSymbol)
    {
        // Get the first syntax reference for the symbol
        var syntaxReference = namedTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault();
    
        if (syntaxReference != null)
        {
            // Get the syntax node and cast it to ClassDeclarationSyntax
            var syntaxNode = syntaxReference.GetSyntax();
            return syntaxNode as ClassDeclarationSyntax;
        }

        return null;
    }
    
    public static ClassDeclarationSyntax GetClassDeclarationSyntax(IMethodSymbol methodSymbol)
    {
        var syntaxReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxReference == null)
        {
            return null;
        }

        var methodSyntax = syntaxReference.GetSyntax() as MethodDeclarationSyntax;
        if (methodSyntax == null)
        {
            return null;
        }

        return methodSyntax.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
    }
}