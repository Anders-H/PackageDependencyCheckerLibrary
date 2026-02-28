#nullable enable

namespace PackageDependencyCheckerLibrary;

internal class ProjectWithFramework
{
    public string ProjectName { get; }
    public string Framework { get; }
    
    public ProjectWithFramework(string projectName, string framework)
    {
        ProjectName = projectName;
        Framework = framework;
    }
}