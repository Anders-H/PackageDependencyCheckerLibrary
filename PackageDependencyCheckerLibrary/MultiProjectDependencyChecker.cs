#nullable enable
using PackageDependencyCheckerLibrary.TreeStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Component = PackageDependencyCheckerLibrary.TreeStructure.Component;

namespace PackageDependencyCheckerLibrary;

public class MultiProjectDependencyChecker
{
    private readonly string _folderName;

    public MultiProjectDependencyChecker(string folderName)
    {
        _folderName = folderName;
    }

    public RootFolder GetTree(out DependencyInfoList data)
    {
        data = GetDependencyInfoList();
        var result = new RootFolder();

        var csProjectFolder = new CsProjectsFolder();
        var projects = csProjectFolder.Load(data);
        result.Add(csProjectFolder);
        
        var componentsFolder = new ComponentsFolder();
        componentsFolder.Load(data);
        result.Add(componentsFolder);

        var frameworksFolder = new FrameworksFolder();
        frameworksFolder.Load(projects);
        result.Add(frameworksFolder);

        return result;
    }

    public DependencyInfoList GetDependencyInfoList()
    {
        var list = new DependencyInfoList();
        var csprojFiles = GetCsprojFiles();
        
        foreach (var dependencies in csprojFiles.Select(csprojFile => new DependencyChecker(csprojFile)))
            list.AddRange(dependencies.GetDependencies().Select(dependency => new DependencyInfo(dependency)));

        foreach (var d in list)
            d.CountProjects(list);

        var result = new DependencyInfoList();
        result.AddRange(list.OrderBy(x => x.PackageName).ThenBy(x => x.PackageVersion).ThenBy(x => x.ProjectName));

        foreach (var d in result)
            _ = d.GetUsagePerVersion(result);

        var comparer = new DependencyInfoPackageComparer();
        var distinctPackages = list.Distinct(comparer);

        foreach (var depInfo in distinctPackages)
        {
            var component = new Component(depInfo.PackageName);
            component.Usage.AddRange(list.Where(x => x.PackageName == depInfo.PackageName));

            foreach (var item in list.Where(item => item.PackageName == depInfo.PackageName))
                item.PackageNameCount = component.Usage.Count;
        }

        var projects = result.GetUniqueProjects();

        foreach (var d in result)
        {
            var framework = d.Framework;
            var count = 0;

            foreach (var project in projects)
            {
                if (d.Framework == project.Framework)
                    count++;
            }

            d.FrameworkCount = count;
        }

        return result;
    }

    public List<string> GetCsprojFiles()
    {
        var dirInfo = new DirectoryInfo(_folderName);
        var csprojFiles = new List<string>();

        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if(fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                csprojFiles.Add(fileInfo.FullName);
        }

        foreach (var directoryInfo in dirInfo.GetDirectories())
            csprojFiles.AddRange(GetCsprojFiles(directoryInfo));

        return csprojFiles;
    }

    private List<string> GetCsprojFiles(DirectoryInfo dirInfo)
    {
        var csprojFiles = new List<string>();

        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if(fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                csprojFiles.Add(fileInfo.FullName);
        }

        foreach (var directoryInfo in dirInfo.GetDirectories())
            csprojFiles.AddRange(GetCsprojFiles(directoryInfo));
        
        return csprojFiles;
    }
}