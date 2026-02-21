#nullable enable
namespace PackageDependencyCheckerLibrary;

public class Dependency
{
    public string SourceFilename { get; set; }
    public string ProjectName { get; }
    public string PackageName { get; }
    public string PackageVersion { get; }

    public Dependency(string sourceFilename, string projectName, string packageName, string packageVersion, string framework)
    {
        SourceFilename = sourceFilename;
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