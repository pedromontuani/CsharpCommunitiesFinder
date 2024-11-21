namespace ModularizationOportunities.utils;

using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MetadataReferenceHelper
{
    public static async Task<List<MetadataReference>> GetMetadataReferencesAsync(Project msProject)
    {
        var references = new List<MetadataReference>();

        var compilation = await msProject.GetCompilationAsync();

        references.AddRange(compilation.References);

        return references;
    }
}