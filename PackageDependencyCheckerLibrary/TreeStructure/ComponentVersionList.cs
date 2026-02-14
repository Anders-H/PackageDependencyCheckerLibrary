using System.Collections.Generic;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class ComponentVersionList : List<ComponentVersion>
{
    public void AddIfNotExists(ComponentVersion version)
    {
        foreach (var v in this)
        {
            if (v.Is(version))
                return;
        }

        Add(version);
    }
}