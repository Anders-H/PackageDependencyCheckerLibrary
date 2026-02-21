#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfoList : List<DependencyInfo>
{
    public int GetUsageCount(string packageName) =>
        this.Count(x => x.PackageName == packageName);

    public int GetProjectCount(DependencyInfo dependencyInfo) =>
        this.Count(x => x.SourceFilename == dependencyInfo.SourceFilename
            && x.Framework == dependencyInfo.Framework);
}