using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModularizationOportunities.dto;
using ModularizationOportunities.enums;
using ModularizationOportunities.utils;

namespace ModularizationOportunities;

public class StructuralCouplingExtraction(IEnumerable<FileClassDeclarations> classDeclarations, Project msProject)
{
    private readonly RelationshipsMatrix _relationshipsGraph = new RelationshipsMatrix();
    private readonly FileClassDeclarations[] _fileClassDeclarations = classDeclarations.ToArray();

    public async Task<RelationshipsMatrix> GetRelationshipsGraph()
    {
        await Parallel.ForEachAsync(_fileClassDeclarations, async (classDeclaration, _) =>
        {
            var classDeclarationSyntax = classDeclaration.classDeclarationSyntax;
            var methodDeclarations =
                classDeclarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            var classTree = classDeclarationSyntax.SyntaxTree;
            var references = await MetadataReferenceHelper.GetMetadataReferencesAsync(msProject);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { classTree }, references);
            var semanticModel = compilation.GetSemanticModel(classTree);

            var syntaxAnalizer = new SyntaxAnalyzer(semanticModel);

            foreach (var method in methodDeclarations)
            {
                var referencedClasses = await GetReferencedClasses(method, syntaxAnalizer);

                if (referencedClasses.Length > 0)
                {
                    AssertRelationship(classDeclarationSyntax, referencedClasses, syntaxAnalizer);
                }
            }
        });

        return _relationshipsGraph;
    }

    private async Task<ClassDeclarationSyntax[]> GetReferencedClasses(MethodDeclarationSyntax method, SyntaxAnalyzer analyzer)
    {
        var referencedClasses = analyzer.GetExplicitlyReferencedClasses(method);
        var calledMethods = analyzer.GetCalledMethods(method);
        var calledMethodsClasses = calledMethods.Select(m => ClassDeclarationSyntaxHelper.GetClassDeclarationSyntax(m)).ToArray();
        var classDeclarations = referencedClasses.Select(r => ClassDeclarationSyntaxHelper.GetClassDeclarationSyntax(r))
            .ToArray();

        var concatenated = classDeclarations.Concat(calledMethodsClasses);
        var filteredTasks = concatenated.Where(c => c != null)
            .Select(async c => new { Class = c, IsPresent = await ProjectUtils.IsClassPresentAsync(msProject, c.Identifier.Text) });

        var filteredResults = await Task.WhenAll(filteredTasks);
        
        return filteredResults.Where(result => result.IsPresent).Select(result => result.Class).ToArray();
    }
    
    private void AssertRelationship(ClassDeclarationSyntax classDeclaration, ClassDeclarationSyntax[] referencedClasses, SyntaxAnalyzer analyzer)
    {
        foreach (var referencedClass in referencedClasses)
        {
            
            if(!_relationshipsGraph.ContainsKey(classDeclaration))
            {
                _relationshipsGraph.TryAdd(classDeclaration, new());
            }
            
            _relationshipsGraph[classDeclaration][referencedClass] = true;

        }
    }
    
    private RelationshipType GetRelationshipType(ClassDeclarationSyntax classDeclaration, ClassDeclarationSyntax referencedClass)
    {
        var classIsEntity = SyntaxAnalyzer.IsEntity(classDeclaration);
        var referencedIsEntity = SyntaxAnalyzer.IsEntity(referencedClass);

        if (!classIsEntity && !referencedIsEntity)
        {
            return RelationshipType.METHOD_TO_METHOD;
        }

        if (classIsEntity && referencedIsEntity)
        {
            return RelationshipType.ENTITY_TO_ENTITY;
        }
        
        return RelationshipType.METHOD_TO_ENTITY;
        
    }
}