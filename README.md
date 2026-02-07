# PackageDependencyCheckerLibrary

A library for extracting Nuget dependencies from a folder.

Uses .NET Standard 2.0, so it works with .NET Framework 4.8 and .NET 8, 9 and 10.

## Example

```
 [TestMethod]
 public void GetDependencyInfoList()
 {
     const string folder = @"D:\GitRepos";
     var x = new MultiProjectDependencyChecker(folder);
     var response = x.GetDependencyInfoList();

     foreach (var r in response)
         System.Diagnostics.Debug.WriteLine(@$"Name: {r.ProjectName} ({r.ProjectNameCount}),
Package: {r.PackageName} ({r.PackageNameCount}), Version: {r.PackageVersion} ({r.PackageVersionCount}),
Framework: {r.Framework} ({r.FrameworkCount})");
    }
```

Output:

```
Name: FantasyGameNameGenerator (1),
Package: BroncoSettingsParser (2), Version: 1.2.0 (2),
Framework: net8.0 (44)
Name: SecretAgentMan (3),
Package: BroncoSettingsParser (2), Version: 1.2.0 (2),
Framework: net8.0-windows (47)
Name: AdventureControlLibrary (3),
Package: CharacterMatrix (1), Version: 1.2.0 (1),
Framework: net8.0-windows (47)
Name: ChessEngineTests (4),
Package: coverlet.collector (14), Version: 1.0.1 (1),
Framework: net8.0-windows (47)
...
```