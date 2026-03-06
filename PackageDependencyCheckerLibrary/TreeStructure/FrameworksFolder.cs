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

    internal void Load(RootFolder projects)
    {
        Frameworks.Clear();
        var d = new FrameworkList();

        foreach (var x in projects)
        {
            if (x is not CsProject p)
                throw new SystemException("No logic here.");

            d.AddIfNotExists(new Framework(p.Framework, p.GetFrameworkCount(projects)));
        }

        Frameworks.AddRange(d.OrderBy(x => x.Name));

        foreach (var f in d.OrderBy(x => x.Name))
            f.Usage.AddRange(projects.Where(x => ((CsProject)x).Framework == f.Name).OrderBy(x => x.Name));
    }
}