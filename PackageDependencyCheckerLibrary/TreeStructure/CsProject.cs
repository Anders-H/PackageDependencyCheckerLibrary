#nullable enable
namespace PackageDependencyCheckerLibrary.TreeStructure;

public class CsProject : INameAndCount
{
    public string Name { get; }
    public int Count => Dependencies.Count;
    public DependencyInfoList Dependencies { get; }

    public CsProject(string name)
    {
        Name = name;
        Dependencies = [];
    }
}