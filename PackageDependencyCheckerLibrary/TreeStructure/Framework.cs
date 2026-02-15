#nullable enable

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class Framework : INameAndCount
{
    public string Name { get; }
    public int Count { get; }
    public DependencyInfoList Usage { get; }

    public Framework(string name, int count)
    {
        Name = name;
        Count = count;
        Usage = [];
    }
}