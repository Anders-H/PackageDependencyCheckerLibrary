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

    public string GetCsv()
    {
        return "hello!";
    }

    public string GetJson()
    {
        return "hello!";
    }

    public string GetXml()
    {
        return "hello!";
    }

    public string GetFixedWidthText()
    {
        return $@"{GetFixedWidthTextHeader()}
{GetFixedWidthTextRows(0, Count)}".Trim();
    }

    public string GetFixedWidthTextHeader()
    {
        var projectNameHeader = "Project name".PadRight(30);
        var projectNameCountHeader = "Count ".PadLeft(7);
        var packageNameHeader = "Package name".PadRight(30);
        var packageNameCountHeader = "Count ".PadLeft(7);
        var packageVersionHeader = "Package version".PadRight(20);
        var packageVersionCountHeader = "Count ".PadLeft(7);
        var frameworkHeader = "Framework".PadRight(20);
        var frameworkCountHeader = "Count ".PadLeft(7);
        return $"{projectNameHeader}{projectNameCountHeader}{packageNameHeader}{packageNameCountHeader}{packageVersionHeader}{packageVersionCountHeader}{frameworkHeader}{frameworkCountHeader}";
    }

    public string GetFixedWidthTextRows(int firstIndex, int count)
    {

    }
}