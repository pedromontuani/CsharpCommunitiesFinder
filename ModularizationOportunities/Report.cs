namespace ModularizationOportunities;

public static class Report
{
    public static void PrintReport(CommunitiesList communities, ClassToNodeMapping classToNode)
    {
        var filteredCommunities = FilterCommunities(communities);
        
        Console.WriteLine($"Encontradas {filteredCommunities.Count} oportunidades de modularização: \n");
        int moduleNum = 0;
        foreach (var community in filteredCommunities)
        {
            Console.WriteLine($"Módulo {++moduleNum}:\n");

            foreach (var node in community)
            {
                var classDeclaration = classToNode.FirstOrDefault(x => x.Value == node).Key;
                Console.WriteLine($"Class: {classDeclaration.Identifier.Text}");
            }
            Console.WriteLine("\n-------------------------------\n");
        }
    }
    
    private static CommunitiesList FilterCommunities(CommunitiesList communities)
    {
        return communities.Where(c => c.Count > 1).ToList();
    }
}