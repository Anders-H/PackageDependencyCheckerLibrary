#nullable enable
using PackageDependencyCheckerLibrary;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PackageDependencies;

public partial class ProjectPropertiesDialog : Form
{
    private readonly ListViewColumnSorter _columnSorter;
    public DependencyInfo? Project { private get; set; }
    public DependencyInfoList? DependencyInfoList { private get; set; }

    public ProjectPropertiesDialog()
    {
        _columnSorter = new ListViewColumnSorter();
        InitializeComponent();
    }

    private void ProjectPropertiesDialog_Load(object sender, EventArgs e)
    {
        if (Project == null || DependencyInfoList == null)
            throw new SystemException(@"Not initialized.");
    }

    private void ProjectPropertiesDialog_Shown(object sender, EventArgs e)
    {
        if (Project == null || DependencyInfoList == null)
            throw new SystemException(@"Not initialized.");

        txtProjectName.Text = Project.ProjectName;
        txtLastUpdate.Text = Project.GetLastUpdateString();
        txtProjectPath.Text = Project.SourceFilename;
        txtFramework.Text = Project.Framework;

        listView1.BeginUpdate();
        listView1.Items.Clear();
        listView1.Columns.Clear();
        listView1.Columns.Add("Package Name", 190);
        listView1.Columns.Add("Total usage", 85, HorizontalAlignment.Center);
        listView1.Columns.Add("Package Version", 100);
        listView1.Columns.Add("Version usage", 85, HorizontalAlignment.Center);
        listView1.Columns.Add("Versions", 85, HorizontalAlignment.Center);

        var theSet = DependencyInfoList
            .Where(x => x.SourceFilename == Project.SourceFilename)
            .OrderBy(x => x.PackageName)
            .ThenBy(x => x.PackageVersion)
            .ToList();

        lblComponents.Text = $@"Components ({theSet.Count}):";

        foreach (var d in theSet)
        {
            var li = new ListViewItem(d.PackageName);
            li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
            li.SubItems.Add(d.PackageVersion);
            li.SubItems.Add(d.GetUsagePerVersion().Count.ToString()).Tag = d.GetUsagePerVersion().Count;
            var numberOfVersions = d.GetNumberOfVersions(DependencyInfoList).Count;
            li.SubItems.Add(numberOfVersions.ToString()).Tag = numberOfVersions;
            li.Tag = d;
            listView1.Items.Add(li);
        }

        listView1.EndUpdate();
    }

    private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        _columnSorter.SortColumn = e.Column;

        if (_columnSorter.SortColumn == _columnSorter.LastSortColumn)
        {
            _columnSorter.SortOrder = _columnSorter.SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
        else
        {
            _columnSorter.SortOrder = SortOrder.Ascending;
        }

        listView1.ListViewItemSorter = _columnSorter;
        listView1.Sort();
        _columnSorter.LastSortColumn = _columnSorter.SortColumn;
    }
}