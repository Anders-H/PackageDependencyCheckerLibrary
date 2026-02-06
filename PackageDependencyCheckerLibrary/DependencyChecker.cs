using System;
using System.Collections.Generic;
using System.IO;

namespace PackageDependencyCheckerLibrary;

public class DependencyChecker
{
    private readonly string _filename;

    public DependencyChecker(string filename)
    {
        _filename = filename;
    }

    public List<Dependency> GetDependencies()
    {
        var dependencies = new List<Dependency>();
        var fileInfo = new FileInfo(_filename);

        if (fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            
            
        }

        return dependencies;
    }
}