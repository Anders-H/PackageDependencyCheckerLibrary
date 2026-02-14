#nullable enable
using PackageDependencyCheckerLibrary.TreeStructure;
using System;
using System.Linq;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfo : Dependency
{
    private ComponentVersionList? _versions;
    public int ProjectNameCount { get; set; }
    public int PackageNameCount { get; set; }
    public int FrameworkCount { get; set; }

    public DependencyInfo(string projectName, string packageName, string packageVersion, string framework) : base(projectName, packageName, packageVersion, framework)
    {
        ProjectNameCount = 0;
        PackageNameCount = 0;
        FrameworkCount = 0;
    }

    public DependencyInfo(Dependency dependency) : this(dependency.ProjectName, dependency.PackageName, dependency.PackageVersion, dependency.Framework)
    {
    }

    public ComponentVersionList GetVersions() =>
    _versions ?? [];

    internal ComponentVersionList GetVersions(DependencyInfoList all)
    {
        if (_versions == null)
        {
            var versions = new ComponentVersionList();

            foreach (var component in all.Where(x => x.PackageName == PackageName))
            {
                var version = component.GetVersion();
                versions.AddIfNotExists(version);
            }

            _versions = [];

            foreach (var version in versions.OrderBy(x => x.Major).ThenBy(x => x.Minor))
                _versions.Add(version);
        }

        return _versions;
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
}