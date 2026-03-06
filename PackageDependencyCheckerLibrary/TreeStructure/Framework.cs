#nullable enable

using System.Collections.Generic;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class Framework : INameAndCount
{
    public string Name { get; }
    public int Count { get; }
    public List<CsProject> Usage { get; }

    public Framework(string name, int count)
    {
        Name = name;
        Count = count;
        Usage = [];
    }
}