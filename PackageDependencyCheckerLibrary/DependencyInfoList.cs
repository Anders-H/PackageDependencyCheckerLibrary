#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfoList : List<DependencyInfo>
{
    public int GetUsageCount(string packageName) =>
        this.Count(x => x.PackageName == packageName);
}