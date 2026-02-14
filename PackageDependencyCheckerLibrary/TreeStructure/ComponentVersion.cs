#nullable enable
namespace PackageDependencyCheckerLibrary.TreeStructure;

public class ComponentVersion
{
    public string Name { get; set; }
    public string VersionString { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }

    public ComponentVersion(string name, string versionString)
    {
        Name = name;
        VersionString = versionString;
        var versionParts = versionString.Split('.');
        
        if (versionParts.Length < 2)
            return;

        int.TryParse(versionParts[0], out var major);
        int.TryParse(versionParts[1], out var minor);
        Major = major;
        Minor = minor;
    }

    public  bool Is(ComponentVersion? other)
    {
        if (other == null)
            return false;

        return Name == other.Name && VersionString == other.VersionString;
    }
}