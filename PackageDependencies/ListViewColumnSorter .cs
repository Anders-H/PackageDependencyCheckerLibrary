using System;
using System.Collections;
using System.Windows.Forms;

namespace PackageDependencies;

public class ListViewColumnSorter : IComparer
{
    public int SortColumn { get; set; }
    public int LastSortColumn { get; set; }
    public SortOrder SortOrder { get; set; }

    public ListViewColumnSorter()
    {
        SortColumn = 0;
        LastSortColumn = -1;
        SortOrder = SortOrder.None;
    }

    public int Compare(object x, object y)
    {
        var response = DoCompare(x, y);

        return SortOrder switch
        {
            SortOrder.Ascending => response,
            SortOrder.Descending => -response,
            _ => 0
        };
    }

    private int DoCompare(object x, object y)
    {
        if (x is not ListViewItem left || y is not ListViewItem right)
            return 0;

        if (SortColumn <= 0)
            return string.Compare(left.Text, right.Text, StringComparison.CurrentCultureIgnoreCase);

        if (left.SubItems[SortColumn].Tag == null || right.SubItems[SortColumn].Tag == null)
            return string.Compare(left.SubItems[SortColumn].Text, right.SubItems[SortColumn].Text, StringComparison.CurrentCultureIgnoreCase);

        if (left.SubItems[SortColumn].Tag is not int leftInt || right.SubItems[SortColumn].Tag is not int rightInt)
            return string.Compare(left.Text, right.Text, StringComparison.CurrentCultureIgnoreCase);

        if (((int?)leftInt)!.Value > ((int?)rightInt)!.Value)
            return 1;
        
        if (leftInt < rightInt)
            return -1;

        return 0;
    }
}