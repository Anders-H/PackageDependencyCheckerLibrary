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

    public int CountProjects()
    {
        if (_projectCount != null)
            return _projectCount.Value;
        
        var s = new StringList();

        foreach (var item in this)
            s.AddIfNotExists(item.ProjectName);

        _projectCount = s.Count;
        return _projectCount.Value;
    }

    public int CountPackages()
    {
        if (_packageCount != null)
            return _packageCount.Value;

        var s = new StringList();

        foreach (var item in this)
            s.AddIfNotExists(item.PackageName);

        _packageCount = s.Count;
        return _packageCount.Value;
    }

    public int CountFrameworks()
    {
        if (_frameworkCount != null)
            return _frameworkCount.Value;

        var s = new StringList();

        foreach (var item in this)
            s.AddIfNotExists(item.Framework);

        _frameworkCount = s.Count;
        return _frameworkCount.Value;
    }
}