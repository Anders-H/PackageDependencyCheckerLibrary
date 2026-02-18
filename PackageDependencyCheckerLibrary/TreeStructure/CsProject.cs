#nullable enable
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class CsProject : INameAndCount
{
    public string Name { get; }
    public int Count => Dependencies.Count;
    public DependencyInfoList Dependencies { get; }
    public string Framework { get; }

    public CsProject(string name, string framework)
    {
        Name = name;
        Dependencies = [];
        Framework = framework;
    }

    public int GetFrameworkCount(CsProjectList projects) =>
        projects.Count(x => x.Framework == Framework);
}