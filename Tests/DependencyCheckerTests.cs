#nullable enable
using PackageDependencyCheckerLibrary;

namespace Tests;

[TestClass]
public sealed class DependencyCheckerTests
{
    [TestMethod]
    public void GetDependencies()
    {
        const string file = @"D:\GitRepos\Secret-Agent-Man\SecretAgentMan\SecretAgentMan\SecretAgentMan.csproj";
        var d = new DependencyChecker(file);
        var response = d.GetDependencies();

        foreach (var r in response)
            System.Diagnostics.Debug.WriteLine($"Name: {r.ProjectName}, Package: {r.PackageName}, Version: {r.PackageVersion}, Framework: {r.Framework}");
    }

    [TestMethod]
    public void Create()
    {
        const string file = @"D:\GitRepos\Secret-Agent-Man\SecretAgentMan\SecretAgentMan\SecretAgentMan.csproj";
        var d = DependencyChecker.Create(file);
        Assert.IsNotNull(d);

        const string wrongFile = @"D:\GitRepos\Secret-Agent-Man\SecretAgentMan\SecretAgentMan\NonExistentFile.vbproj";
        d = DependencyChecker.Create(wrongFile);
        Assert.IsNull(d);

        const string fileNotFound = @"D:\GitRepos\Secret-Agent-Man\SecretAgentMan\SecretAgentMan\NonExistentFile.csproj";
        d = DependencyChecker.Create(fileNotFound);
        Assert.IsNull(d);
    }

    [TestMethod]
    public void GetCsprojFiles()
    {
        const string folder = @"D:\GitRepos";
        var x = new MultiProjectDependencyChecker(folder);
        var files = x.GetCsprojFiles();

        foreach (var file in files)
            System.Diagnostics.Debug.WriteLine(file);
    }

    [TestMethod]
    public void GetDependencyInfoList()
    {
        const string folder = @"D:\GitRepos";
        var x = new MultiProjectDependencyChecker(folder);
        var response = x.GetDependencyInfoList();
        
        foreach (var r in response)
            System.Diagnostics.Debug.WriteLine(@$"Name: {r.ProjectName} ({r.ProjectNameCount}),
Package: {r.PackageName} ({r.PackageNameCount}), Version: {r.PackageVersion} ({r.GetUsagePerVersion().Count}),
Framework: {r.Framework} ({r.FrameworkCount})");
    }

    [TestMethod]
    public void SaveDependencyInfoList()
    {
        const string folder = @"D:\GitRepos";
        var x = new MultiProjectDependencyChecker(folder);
        var response = x.GetDependencyInfoList();
        response.SaveFixedWidthText(@"C:\Users\hbom\Desktop\testrapport.txt");
    }
}