#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class FrameworkList : List<Framework>
{
    public void AddIfNotExists(Framework framework)
    {
        foreach (var f in this)
        {
            if (f.Name == framework.Name)
                return;
        }

        Add(framework);
    }

    public int GetCount(DependencyInfo dependencyInfo) =>
        (from f in this where f.Name == dependencyInfo.Framework select f.Count).FirstOrDefault();
}