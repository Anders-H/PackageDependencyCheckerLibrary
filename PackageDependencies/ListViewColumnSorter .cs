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
        var left = x as ListViewItem;
        var right = y as ListViewItem;

        if (left == null || right == null)
            return 0;

        if (SortColumn <= 0)
            return string.Compare(left.Text, right.Text, StringComparison.CurrentCultureIgnoreCase);

        if (left.SubItems[SortColumn].Tag == null || right.SubItems[SortColumn].Tag == null)
            return string.Compare(left.SubItems[SortColumn].Text, right.SubItems[SortColumn].Text, StringComparison.CurrentCultureIgnoreCase);

        var leftInt = left.SubItems[SortColumn].Tag as int?;
        var rightInt = right.SubItems[SortColumn].Tag as int?;

        if (!leftInt.HasValue || !rightInt.HasValue)
            return string.Compare(left.Text, right.Text, StringComparison.CurrentCultureIgnoreCase);

        if (leftInt!.Value > rightInt!.Value)
            return 1;
        
        if (leftInt.Value < rightInt.Value)
            return -1;

        return 0;
    }

}