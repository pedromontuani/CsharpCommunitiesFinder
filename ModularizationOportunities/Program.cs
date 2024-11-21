// See https://aka.ms/new-console-template for more information

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using ModularizationOportunities.utils;

namespace ModularizationOportunities;
    
static class Program
{
    static async Task Main(string[] args)
    {
        CheckArgs(args);
        string projectPath = args[0];

        CheckPath(projectPath);
        
        InitializeMsBuild();
        
        var project = await OpenProject(projectPath);
        
        var analyzer = new Analyzer(project);
        await analyzer.Analyze();
        
        var classToNode = analyzer.GetClassToNodeMapping();
        var communities = analyzer.GetCommunities();
        
        Report.PrintReport(communities, classToNode);
    }
    
    private static void CheckArgs(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Caminho do diretório não informado");
            Environment.Exit(1);
        }
        
    }
    
    private static void CheckPath(string path)
    {
        if (!FilesManager.IsValidPath(path))
        {
            Console.WriteLine($"Caminho inválido");
            Environment.Exit(1);
        }
    }
    
    private static void InitializeMsBuild()
    {
        MSBuildLocator.RegisterDefaults();
        
        if(!MSBuildLocator.IsRegistered)
        {
            var msBuildPath = Environment.GetEnvironmentVariable("MSBUILD_PATH");
            
            if (String.IsNullOrEmpty(msBuildPath))
            {
                Console.WriteLine("Forneca o caminho do MSBuild como variável de ambiente MSBUILD_PATH.");
                return;
            }
            
            MSBuildLocator.RegisterMSBuildPath(msBuildPath);
        }
    }
    
    private static async Task<Project> OpenProject(string projectPath)
    {
        var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { { "AlwaysCompileMarkupFilesInSeparateDomain", "true" }, { "CheckForSystemRuntimeDependency", "true" } });
        var project = await workspace.OpenProjectAsync(projectPath).ConfigureAwait(false);
        return project;
    }
}