#nullable enable
namespace PackageDependencyCheckerLibrary;

public class DependencyInfo : Dependency
{
    public int ProjectNameCount { get; set; }
    public int PackageNameCount { get; set; }
    public int PackageVersionCount { get; set; }
    public int FrameworkCount { get; set; }

    public DependencyInfo(string projectName, string packageName, string packageVersion, string framework) : base(projectName, packageName, packageVersion, framework)
    {
        ProjectNameCount = 0;
        PackageNameCount = 0;
        PackageVersionCount = 0;
        FrameworkCount = 0;
    }

    public DependencyInfo(Dependency dependency) : this(dependency.ProjectName, dependency.PackageName, dependency.PackageVersion, dependency.Framework)
    {
    }
}