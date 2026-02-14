#nullable enable
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class Component : INameAndCount
{
    private ComponentVersionList? _versions;
    public string Name { get; }
    public int Count => Usage.Count;
    public DependencyInfoList Usage { get; }
    
    public Component(string name)
    {
        Name = name;
        Usage = [];
    }

    public ComponentVersionList GetVersions()
    {
        if (_versions == null)
        {
            var versions = new ComponentVersionList();

            foreach (var component in Usage)
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
}