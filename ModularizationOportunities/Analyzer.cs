using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModularizationOportunities.dto;

namespace ModularizationOportunities;

public class Analyzer(Project msProject)
{
    private CsFile[] _projectClasses;
    private readonly ClassToNodeMapping _classToNode = new ();
    private CommunitiesList _communities;

    private async Task GetProjectClasses()
    {
        var tasks = msProject.Documents.Select(async d => new CsFile(d,
            (await d.GetSyntaxTreeAsync()))).ToArray();
        
        _projectClasses = await Task.WhenAll(tasks) ;
    }

    public async Task Analyze()
    {
       await GetProjectClasses();
       var classDeclarations = _projectClasses.SelectMany(c => c.classDeclarations).ToArray();
       var couplingExtraction = new StructuralCouplingExtraction(classDeclarations, msProject);
       var relationshipsGraph = await couplingExtraction.GetRelationshipsGraph();
       
       var graph = GenerateGraph(relationshipsGraph);
       _communities = FindCommunities(graph);
    }
    
    public ClassToNodeMapping GetClassToNodeMapping()
    {
        return _classToNode;
    }
    
    public CommunitiesList GetCommunities()
    {
        return _communities;
    }

    private Graph GenerateGraph(RelationshipsMatrix matrix)
    {
        var graph = new Graph();
        int nodeId = 0;

        foreach (var classDeclaration in matrix.Keys)
        {
            if (!_classToNode.ContainsKey(classDeclaration))
            {
                _classToNode[classDeclaration] = nodeId;
                graph.AddNode(nodeId++);
            }

            foreach (var referenced in matrix[classDeclaration].Keys)
            {
                if (!_classToNode.ContainsKey(referenced))
                {
                    _classToNode[referenced] = nodeId;
                    graph.AddNode(nodeId++);
                }
                graph.AddEdge(_classToNode[classDeclaration], _classToNode[referenced]);
            }
        }

        return graph;
    }
    
    private CommunitiesList FindCommunities(Graph graph)
    {
        var leiden = new LeidenAlgorithm(graph);
        var communities = leiden.FindCommunities();

        return communities;
    }
    
}