using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModularizationOportunities.utils;

namespace ModularizationOportunities.dto;

public class FileClassDeclarations(CsFile file, ClassDeclarationSyntax classDeclaration)
{
    public CsFile parent { get; } = file;
    public ClassDeclarationSyntax classDeclarationSyntax { get;  } = classDeclaration;
}

public class CsFile
{
    public Document document { get; }
    public SyntaxTree syntaxTree { get; }
    public FileClassDeclarations[] classDeclarations { get; }

    public CsFile(Document document, SyntaxTree syntaxTree)
    {
        this.document = document;
        this.syntaxTree = syntaxTree;
        classDeclarations = GetClassDeclarations()
            .Select(c => new FileClassDeclarations(this, c))
            .ToArray();
    }
    
    private List<ClassDeclarationSyntax> GetClassDeclarations()
    {
        return syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
    }
}