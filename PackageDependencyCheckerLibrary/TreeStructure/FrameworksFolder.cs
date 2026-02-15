#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class FrameworksFolder : INameAndCount
{
    public string Name => "Frameworks";
    public int Count => Frameworks.Count;
    public FrameworkList Frameworks { get; }

    public FrameworksFolder()
    {
        Frameworks = [];
    }

    internal void Load(DependencyInfoList list)
    {
        Frameworks.Clear();
        var d = new FrameworkList();

        foreach (var l in list)
            d.AddIfNotExists(new Framework(l.Framework, l.FrameworkCount));

        Frameworks.AddRange(d.OrderBy(x => x.Name));

        foreach (var f in d.OrderBy(x => x.Name))
        {
            f.Usage.AddRange(list.Where(x => x.Framework == f.Name).OrderBy(x => x.ProjectName));
        }
    }
}