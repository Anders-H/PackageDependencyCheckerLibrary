#nullable enable
namespace PackageDependencyCheckerLibrary;

public class DependencyInfo : Dependency
{
    public int ProjectNameCount { get; set; }
    public int PackageNameCount { get; set; }
    public int FrameworkCount { get; set; }

    public DependencyInfo(string projectName, string packageName, string packageVersion, string framework) : base(projectName, packageName, packageVersion, framework)
    {
        ProjectNameCount = 0;
        PackageNameCount = 0;
        FrameworkCount = 0;
    }
}