#nullable enable
using System.Collections.Generic;

namespace PackageDependencyCheckerLibrary;

internal class DependencyInfoPackageComparer : IEqualityComparer<DependencyInfo>
{
    public bool Equals(DependencyInfo? x, DependencyInfo? y)
    {
        if (x == null && y == null)
            return true;

        if (x is null || y is null)
            return false;

        return x.PackageName == y.PackageName;
    }
    public int GetHashCode(DependencyInfo obj) =>
        obj.PackageName.GetHashCode();
}