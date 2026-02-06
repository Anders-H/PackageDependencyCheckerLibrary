#nullable enable
using System.Collections.Generic;
using System.IO;

namespace PackageDependencyCheckerLibrary;

public class MultiProjectDependencyChecker
{
    private readonly string _folderName;

    public MultiProjectDependencyChecker(string folderName)
    {
        _folderName = folderName;
    }

    public DependencyInfoList GetDependencyInfoList()
    {
        var dirInfo = new DirectoryInfo(_folderName);
        var csprojFiles = GetCsprojFiles();
        return null;
    }

    public List<string> GetCsprojFiles()
    {
        var dirInfo = new DirectoryInfo(_folderName);
        var csprojFiles = new List<string>();

        foreach (var fileInfo in dirInfo.GetFiles())
        {
            if(fileInfo.Extension.Equals(".csproj", System.StringComparison.OrdinalIgnoreCase))
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
            if(fileInfo.Extension.Equals(".csproj", System.StringComparison.OrdinalIgnoreCase))
                csprojFiles.Add(fileInfo.FullName);
        }

        foreach (var directoryInfo in dirInfo.GetDirectories())
            csprojFiles.AddRange(GetCsprojFiles(directoryInfo));
        
        return csprojFiles;
    }
}