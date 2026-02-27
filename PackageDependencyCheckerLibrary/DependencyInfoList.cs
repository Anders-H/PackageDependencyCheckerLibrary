#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PackageDependencyCheckerLibrary;

public class DependencyInfoList : List<DependencyInfo>
{
    private const int ProjectNameLength = 23;
    private const int ProjectNameCountLength = 7;
    private const int PackageNameLength = 23;
    private const int PackageNameCountLength = 7;
    private const int PackageVersionLength = 17;
    private const int PackageVersionCountLength = 7;
    private const int FrameworkLength = 15;
    private const int FrameworkCountLength = 7;

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
{GetSeparator('=')}
{GetFixedWidthTextRows(0, Count)}".Trim();
    }

    public bool SaveFixedWidthText(string targetFilename)
    {
        try
        {
            File.WriteAllText(targetFilename, GetFixedWidthText());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFixedWidthTextHeader()
    {
        var projectNameHeader = "Project name".PadRight(ProjectNameLength);
        var projectNameCountHeader = "Count ".PadLeft(ProjectNameCountLength);
        var packageNameHeader = "Package name".PadRight(PackageNameLength);
        var packageNameCountHeader = "Count ".PadLeft(PackageNameCountLength);
        var packageVersionHeader = "Package version".PadRight(PackageVersionLength);
        var packageVersionCountHeader = "Count ".PadLeft(PackageVersionCountLength);
        var frameworkHeader = "Framework".PadRight(FrameworkLength);
        var frameworkCountHeader = "Count ".PadLeft(FrameworkCountLength);
        return $"{projectNameHeader}{projectNameCountHeader}{packageNameHeader}{packageNameCountHeader}{packageVersionHeader}{packageVersionCountHeader}{frameworkHeader}{frameworkCountHeader}".Trim();
    }

    public string GetSeparator(char character) =>
        new(character, ProjectNameLength + ProjectNameCountLength + PackageNameLength + PackageNameCountLength + PackageVersionLength + PackageVersionCountLength + FrameworkLength + FrameworkCountLength - 1);

    public string GetFixedWidthTextRows(int firstIndex, int count)
    {
        var s = new StringBuilder();
        var rows = GetFixedWidthTextRowsAsList(firstIndex, count);
        
        foreach (var row in rows)
            s.AppendLine(row);
        
        return s.ToString();
    }

    public List<string> GetFixedWidthTextRowsAsList(int firstIndex, int count)
    {
        var s = new List<string>();

        for (var i = firstIndex; i < firstIndex + count && i < Count; i++)
        {
            var d = this[i];
            var projectName = d.ProjectName.Length >= ProjectNameLength
                ? d.ProjectName.Substring(0, ProjectNameLength - 1)
                : d.ProjectName;

            projectName = projectName.PadRight(ProjectNameLength);

            var projectNameCount = $"{d.ProjectNameCount} ".PadLeft(ProjectNameCountLength);

            var packageName = d.PackageName.Length >= PackageNameLength
                ? d.PackageName.Substring(0, PackageNameLength - 1)
                : d.PackageName;

            packageName = packageName.PadRight(PackageNameLength);

            var packageNameCount = $"{d.PackageNameCount} ".PadLeft(PackageNameCountLength);

            var packageVersion = d.PackageVersion.Length >= PackageVersionLength
                ? d.PackageVersion.Substring(0, PackageVersionLength - 1)
                : d.PackageVersion;

            packageVersion = packageVersion.PadRight(PackageVersionLength);

            var packageVersionCount = $"{d.GetUsagePerVersion().Count} ".PadLeft(PackageVersionCountLength);

            var framework = d.Framework.Length >= FrameworkLength
                ? d.Framework.Substring(0, FrameworkLength - 1)
                : d.Framework;

            framework = framework.PadRight(FrameworkLength);

            var frameworkCount = $"{d.FrameworkCount} ".PadLeft(FrameworkCountLength);

            s.Add($"{projectName}{projectNameCount}{packageName}{packageNameCount}{packageVersion}{packageVersionCount}{framework}{frameworkCount}");
        }

        return s;
    }
}