using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModularizationOportunities.dto;

namespace ModularizationOportunities;

public class Analyzer
{
    private CsFile[] _projectClasses;
    private Project _msProject;
    
    public Analyzer(Project msProject)
    {
       _msProject = msProject;
    }
    
    private async Task GetProjectClasses()
    {
        _projectClasses = await Task.WhenAll(_msProject.Documents.Select(async d => new CsFile(d,
            (await d.GetSyntaxTreeAsync()))).ToArray()) ;
    }

    public async Task Analyze()
    {
       await GetProjectClasses();
       var classDeclarations = _projectClasses.SelectMany(c => c.classDeclarations).ToArray();
       var couplingExtraction = new StructuralCouplingExtraction(classDeclarations, _msProject);
       var relationshipsGraph = await couplingExtraction.GetRelationshipsGraph();

       DisplayCallMatrix(relationshipsGraph);
       DisplayComunities(relationshipsGraph);
    }

    private void DisplayCallMatrix(RelationshipsMatrix matrix)
    {
        foreach (var classDeclaration in matrix.Keys)
        {
            Console.WriteLine("");
            Console.WriteLine("--------");
            Console.WriteLine($"Class: {classDeclaration.Identifier.Text}");
            foreach (var referenced in matrix[classDeclaration].Keys)
            {
                Console.WriteLine($"Reference: {referenced.Identifier.Text}");
            }
            Console.WriteLine("--------");
        }
    }
    
    private void DisplayComunities(RelationshipsMatrix matrix)
    {
        var graph = new Graph();
        var classToNode = new Dictionary<ClassDeclarationSyntax, int>();
        int nodeId = 0;

        foreach (var classDeclaration in matrix.Keys)
        {
            if (!classToNode.ContainsKey(classDeclaration))
            {
                classToNode[classDeclaration] = nodeId;
                graph.AddNode(nodeId++);
            }

            foreach (var referenced in matrix[classDeclaration].Keys)
            {
                if (!classToNode.ContainsKey(referenced))
                {
                    classToNode[referenced] = nodeId;
                    graph.AddNode(nodeId++);
                }
                graph.AddEdge(classToNode[classDeclaration], classToNode[referenced]);
            }
        }
        
        var leiden = new LeidenAlgorithm(graph);
        var communities = leiden.FindCommunities();
        
        
        foreach (var community in communities)
        {
            Console.WriteLine("Community:");
            foreach (var node in community)
            {
                var classDeclaration = classToNode.FirstOrDefault(x => x.Value == node).Key;
                Console.WriteLine($"Class: {classDeclaration.Identifier.Text}");
            }
            Console.WriteLine("--------");
        }
    }

    
    
}