#nullable enable
namespace PackageDependencyCheckerLibrary;

public class Dependency
{
    public string ProjectName { get; }
    public string PackageName { get; }
    public string PackageVersion { get; }

    public Dependency(string projectName, string packageName, string packageVersion, string framework)
    {
        ProjectName = projectName;
        PackageName = packageName;
        PackageVersion = packageVersion;
        Framework = framework;
    }

    public string Framework
    {
        get;
        set => field = value.StartsWith(".") ? value.Substring(1) : value;
    }
}