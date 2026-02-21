#nullable enable
using System.IO;
using PackageDependencyCheckerLibrary.TreeStructure;
using System.Linq;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfo : Dependency
{
    private ComponentVersionList? _usagePerVersion;
    public int ProjectNameCount { get; set; }
    public int PackageNameCount { get; set; }
    public int FrameworkCount { get; set; }

    public DependencyInfo(string sourceFilename, string projectName, string packageName, string packageVersion, string framework) : base(sourceFilename, projectName, packageName, packageVersion, framework)
    {
        ProjectNameCount = 0;
        PackageNameCount = 0;
        FrameworkCount = 0;
    }

    public DependencyInfo(Dependency dependency) : this(dependency.SourceFilename, dependency.ProjectName, dependency.PackageName, dependency.PackageVersion, dependency.Framework)
    {
    }

    public ComponentVersionList GetUsagePerVersion() =>
        _usagePerVersion ?? [];

    internal ComponentVersionList GetUsagePerVersion(DependencyInfoList all)
    {
        if (_usagePerVersion != null)
            return _usagePerVersion;

        var versions = new ComponentVersionList();

        foreach (var component in all.Where(x => x.PackageName == PackageName))
        {
            var version = component.GetVersion();
            versions.AddIfNotExists(version);
        }

        _usagePerVersion = [];

        foreach (var version in versions.OrderBy(x => x.Major).ThenBy(x => x.Minor))
            _usagePerVersion.Add(version);

        return _usagePerVersion;
    }

    public ComponentVersion GetVersion()
    {
        try
        {
            return new ComponentVersion(PackageName, PackageVersion);
        }
        catch
        {
            return new ComponentVersion(PackageName, "1.0");
        }
    }

    public void CountProjects(DependencyInfoList list)
    {
        var s = new StringList();

        foreach (var dependency in list)
        {
            if (dependency.PackageName == PackageName)
                s.Add(dependency.PackageName);
        }

        ProjectNameCount = s.Count;
    }

    public string GetLastUpdateString()
    {
        try
        {
            var fi = new FileInfo(SourceFilename);

            if (!fi.Exists)
                return "File not found.";

            var d = fi.LastWriteTime;
            return d.ToString("yyyy-MM-dd (HH:mm)");
        }
        catch
        {
            return "Failed to get last update date.";
        }
    }

    public DependencyInfoList GetNumberOfVersions(DependencyInfoList all)
    {
        var result = new DependencyInfoList();

        foreach (var component in all)
        {

        }

        return result;
    }
}