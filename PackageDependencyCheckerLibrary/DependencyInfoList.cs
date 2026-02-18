#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfoList : List<DependencyInfo>
{
    private int? _projectCount;
    private int? _packageCount;
    private int? _frameworkCount;

    public int GetUsageCount(string packageName) =>
        this.Count(x => x.PackageName == packageName);
}