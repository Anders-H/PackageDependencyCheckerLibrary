#nullable enable
namespace PackageDependencyCheckerLibrary.TreeStructure;

public class Component : INameAndCount
{
    public string Name { get; }
    public int Count => Usage.Count;
    public DependencyInfoList Usage { get; }

    public Component(string name)
    {
        Name = name;
        Usage = [];
    }
}