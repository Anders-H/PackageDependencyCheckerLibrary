#nullable enable
using PackageDependencyCheckerLibrary.TreeStructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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
        var s = new StringBuilder();
        s.AppendLine("Project name; Count; Package name; Count; Package version; Count; Framework; Count");

        foreach (var d in this)
        {
            var row = $"{d.ProjectName.Replace(';', ',')}; {d.ProjectNameCount}; {d.PackageName.Replace(';', ',')}; {d.PackageNameCount}; {d.PackageVersion.Replace(';', ',')}; {d.PackageVersionCount}; {d.Framework.Replace(';', ',')}; {d.FrameworkCount}";

            if (d == this.Last())
                s.Append(row);
            else
                s.AppendLine(row);
        }

        return s.ToString();
    }

    public string GetXml()
    {
        var s = new StringBuilder();
        s.AppendLine(@"<?xml version = ""1.0"" encoding = ""UTF-8"" standalone = ""yes"" ?>");
        s.AppendLine("<packages>");

        var currentPackageName = "";
        var packageIsOpen = false;
        var currentPackageVersion = "";
        var versionIsOpen = false;

        foreach (var p in this.OrderBy(x => x.PackageName).ThenBy(x => x.PackageVersion))
        {
            if (p.PackageName != currentPackageName)
            {
                currentPackageName = p.PackageName;
                currentPackageVersion = "";

                if (packageIsOpen)
                {
                    if (versionIsOpen)
                    {
                        versionIsOpen = false;
                        s.AppendLine("        </version>");
                    }

                    packageIsOpen = false;
                    s.AppendLine("    </package>");
                }

                packageIsOpen = true;
                s.AppendLine($@"    <package value=""{p.PackageName}"" count=""{p.PackageNameCount}"">");
            }

            if (p.PackageVersion != currentPackageVersion)
            {
                currentPackageVersion = p.PackageVersion;

                if (versionIsOpen)
                    s.AppendLine("        </version>");

                versionIsOpen = true;
                s.AppendLine($@"        <version value=""{p.PackageVersion}"" count=""{p.PackageVersionCount}"">");
            }

            s.AppendLine($@"            <project count=""{p.ProjectNameCount}"">");
            s.AppendLine($"                <name>{HttpUtility.HtmlDecode(p.ProjectName)}</name>");
            s.AppendLine($@"                <framework count=""{p.FrameworkCount}"">{HttpUtility.HtmlDecode(p.Framework)}</framework>");
            s.AppendLine("            </project>");
        }

        if (versionIsOpen)
            s.AppendLine("        </version>");

        if (packageIsOpen)
            s.AppendLine("    </package>");

        s.AppendLine("</packages>");
        return s.ToString();
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

            var packageVersionCount = $"{d.PackageVersionCount} ".PadLeft(PackageVersionCountLength);

            var framework = d.Framework.Length >= FrameworkLength
                ? d.Framework.Substring(0, FrameworkLength - 1)
                : d.Framework;

            framework = framework.PadRight(FrameworkLength);

            var frameworkCount = $"{d.FrameworkCount} ".PadLeft(FrameworkCountLength);

            s.Add($"{projectName}{projectNameCount}{packageName}{packageNameCount}{packageVersion}{packageVersionCount}{framework}{frameworkCount}");
        }

        return s;
    }

    internal List<ProjectWithFramework> GetUniqueProjects()
    {
        var projects = new List<ProjectWithFramework>();

        foreach (var d in this)
        {
            var project = new ProjectWithFramework(d.ProjectName, d.Framework);
            
            if (!projects.Any(x => x.ProjectName == project.ProjectName && x.Framework == project.Framework))
                projects.Add(project);
        }

        return projects;
    }

    public DependencyInfo? GetFirstProject(CsProject csProject) =>
        this.FirstOrDefault(d => d.ProjectName == csProject.Name && d.Framework == csProject.Framework);
}