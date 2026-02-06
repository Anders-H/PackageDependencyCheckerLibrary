#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PackageDependencyCheckerLibrary;

public class DependencyChecker
{
    private readonly string _filename;

    public DependencyChecker(string filename)
    {
        _filename = filename;
    }

    public static DependencyChecker? Create(string filename)
    {
        var fileInfo = new FileInfo(filename);

        if (!fileInfo.Exists)
            return null;

        return fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase)
            ? new DependencyChecker(filename)
            : null;
    }

    public List<Dependency> GetDependencies()
    {
        var dependencies = new List<Dependency>();
        var fileInfo = new FileInfo(_filename);

        if (fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            var nameOnly = fileInfo.Name.Substring(0, fileInfo.Name.Length - 7);
            var framework = "";
            var contents = $@"<?xml version = ""1.0"" encoding = ""UTF-8"" standalone = ""yes"" ?>
{File.ReadAllText(fileInfo.FullName)}
";
            var dom = new XmlDocument();
            dom.LoadXml(contents);
            var doc = dom.DocumentElement;

            if (doc == null)
                throw new SystemException("Document element not found.");

            if (doc.Name != "Project")
                throw new SystemException("Root element is not 'Project'.");

            foreach (XmlElement element in doc.ChildNodes)
            {
                if (element.Name == "PropertyGroup" && framework == "")
                {
                    foreach (XmlElement item in element.ChildNodes)
                    {
                        if (item.Name != "TargetFramework" || framework != "")
                            continue;

                        framework = item.InnerText;
                        break;
                    }
                }

                if (element.Name != "ItemGroup")
                    continue;

                foreach (XmlElement item in element.ChildNodes)
                {
                    if (item.Name != "PackageReference")
                        continue;

                    string include;
                    string version;

                    try
                    {
                        include = item.Attributes.GetNamedItem("Include")?.Value ?? "";
                    }
                    catch
                    {
                        include = "[Read error]";
                    }

                    try
                    {
                        version = item.Attributes.GetNamedItem("Version")?.Value ?? "";
                    }
                    catch
                    {
                        version = "[Read error]";
                    }

                    dependencies.Add(new Dependency(nameOnly, include, version, framework));
                }
            }

            if (!string.IsNullOrWhiteSpace(framework))
            {
                foreach (var dependency in dependencies.Where(dependency => string.IsNullOrWhiteSpace(dependency.Framework)))
                    dependency.Framework = framework;
            }
        }

        return dependencies;
    }
}