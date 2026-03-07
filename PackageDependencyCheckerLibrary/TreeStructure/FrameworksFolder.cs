#nullable enable
using System;
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

    internal void Load(List<CsProject> projects)
    {
        Frameworks.Clear();
        var d = new FrameworkList();

        foreach (var x in projects)
            d.AddIfNotExists(new Framework(x.Framework, x.GetFrameworkCount(projects)));

        Frameworks.AddRange(d.OrderBy(x => x.Name));

        foreach (var f in d.OrderBy(x => x.Name))
            f.Usage.AddRange(projects.Where(x => x.Framework == f.Name).OrderBy(x => x.Name));
    }
}