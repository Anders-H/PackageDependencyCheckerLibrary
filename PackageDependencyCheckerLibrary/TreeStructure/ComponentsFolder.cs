#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class ComponentsFolder : INameAndCount
{
    public string Name => "Components";
    public int Count => Components.Count;
    public ComponentList Components { get; }

    public ComponentsFolder()
    {
        Components = [];
    }

    internal class DependencyInfoComparer : IEqualityComparer<DependencyInfo>
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

    internal void Load(DependencyInfoList list)
    {
        Components.Clear();
        var comparer = new DependencyInfoComparer();
        var d = list.Distinct(comparer);

        foreach (var depInfo in d.OrderBy(x => x.PackageName))
        {
            var component = new Component(depInfo.PackageName);
            component.Usage.AddRange(list.Where(x => x.PackageName == depInfo.PackageName));
            Components.Add(component);
        }
    }
}