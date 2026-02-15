#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace PackageDependencyCheckerLibrary.TreeStructure;

public class CsProjectsFolder : INameAndCount
{
    public string Name => "CS Projects";
    public int Count => CsProjects.Count;
    public CsProjectList CsProjects { get; }

    public CsProjectsFolder()
    {
        CsProjects = [];
    }

    private class DependencyInfoProjectComparer : IEqualityComparer<DependencyInfo>
    {
        public bool Equals(DependencyInfo? x, DependencyInfo? y)
        {
            if (x == null && y == null)
                return true;

            if (x is null || y is null)
                return false;

            return x.ProjectName == y.ProjectName;
        }
        public int GetHashCode(DependencyInfo obj) =>
            obj.ProjectName.GetHashCode();
    }

    internal void Load(DependencyInfoList list)
    {
        CsProjects.Clear();
        var comparer = new DependencyInfoProjectComparer();
        var d = list.Distinct(comparer);

        foreach (var depInfo in d.OrderBy(x => x.ProjectName))
        {
            var csProject = new CsProject(depInfo.ProjectName);
            csProject.Dependencies.AddRange(list.Where(x => x.ProjectName == depInfo.ProjectName));
            CsProjects.Add(csProject);
        }
    }
}