#nullable enable
using System.Collections.Generic;

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
}