#nullable enable
using System.Collections.Generic;

namespace PackageDependencyCheckerLibrary;

public class StringList : List<string>
{
    public void AddIfNotExists(string value)
    {
        foreach (var v in this)
        {
            if (v == value)
                return;
        }

        Add(value);
    }
}