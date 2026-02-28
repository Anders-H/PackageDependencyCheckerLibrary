#nullable enable
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

    internal void Load(DependencyInfoList list)
    {
        Components.Clear();
        var comparer = new DependencyInfoPackageComparer();
        var d = list.Distinct(comparer);

        foreach (var depInfo in d.OrderBy(x => x.PackageName))
        {
            var component = new Component(depInfo.PackageName);
            component.Usage.AddRange(list.Where(x => x.PackageName == depInfo.PackageName));
            Components.Add(component);
        }
    }
}