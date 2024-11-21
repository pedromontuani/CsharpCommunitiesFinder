using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModularizationOportunities.utils;

public static class ProjectUtils
{
    public static async Task<bool> IsClassPresentAsync(Project project, string className)
    {
        foreach (var document in project.Documents)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync();
            if (syntaxTree == null)
            {
                continue;
            }

            var root = await syntaxTree.GetRootAsync();
            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            if (classDeclarations.Any(cd => cd.Identifier.Text == className))
            {
                return true;
            }
        }

        return false;
    }
}