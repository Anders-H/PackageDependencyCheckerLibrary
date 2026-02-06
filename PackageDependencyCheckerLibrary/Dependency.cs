namespace PackageDependencyCheckerLibrary;

public class Dependency
{
    public string ProjectName { get; set; }
    public string PackageName { get; set; }
    public string PackageVersion { get; set; }
    public string Framework { get; set; }
}