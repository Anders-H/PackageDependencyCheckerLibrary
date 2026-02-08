namespace PackageDependencyCheckerLibrary.TreeStructure;

public class Framework : INameAndCount
{
    public string Name { get; }
    public int Count { get; }
    public CsProjectList CsProjects { get; }

    public Framework(string name, int count)
    {
        Name = name;
        Count = count;
        CsProjects = new CsProjectList();
    }
}