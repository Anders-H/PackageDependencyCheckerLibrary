#nullable enable
using PackageDependencyCheckerLibrary;
using PackageDependencyCheckerLibrary.TreeStructure;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PackageDependencies;

public partial class MainWindow : Form
{
    private readonly ListViewColumnSorter _columnSorter;
    private DependencyInfoList _data;
    public const string ApplicationName = "Package Dependencies";
    
    public MainWindow()
    {
        _columnSorter = new ListViewColumnSorter();
        _data = [];
        InitializeComponent();
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {
#if DEBUG
        var d = new MultiProjectDependencyChecker(@"D:\GitRepos");
        var root = d.GetTree(out var data);
        _data = data;
        Text = $@"{ApplicationName} - D:\GitRepos";
        RefreshView(root);
#endif
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var f = new FolderBrowserDialog();
        f.RootFolder = Environment.SpecialFolder.MyComputer;
        f.Description = @"Select the folder to analyze";
        var response = f.ShowDialog(this);

        if (response != DialogResult.OK)
            return;

        RootFolder root;
        Cursor = Cursors.WaitCursor;

        try
        {
            var d = new MultiProjectDependencyChecker(f.SelectedPath);
            root = d.GetTree(out var data);
            _data = data;
        }
        catch (Exception exception)
        {
            Cursor = Cursors.Default;
            MessageBox.Show(this, $@"An error occurred while analyzing the folder: {exception.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Cursor = Cursors.Default;
        Text = $@"{ApplicationName} - {f.SelectedPath}";
        RefreshView(root);
    }

    private void RefreshView(RootFolder root)
    {
        Cursor = Cursors.WaitCursor;
        treeView1.BeginUpdate();
        treeView1.Nodes.Clear();

        foreach (var item in root)
        {
            var node = new TreeNode($"{item.Name} ({item.Count})");
            node.Tag = item;
            treeView1.Nodes.Add(node);

            if (item.Count > 0)
                AddChildren(node, item);
        }

        treeView1.EndUpdate();

        if (treeView1.Nodes.Count <= 0)
        {
            Cursor = Cursors.Default;
            return;
        }

        treeView1.SelectedNode = treeView1.Nodes[0];
        treeView1.SelectedNode.EnsureVisible();
        Cursor = Cursors.Default;
    }

    private void AddChildren(TreeNode node, INameAndCount item)
    {
        if (item is CsProjectsFolder csProjectsFolder)
        {
            foreach (var csProject in csProjectsFolder.CsProjects)
            {
                var childNode = new TreeNode($"{csProject.Name} ({csProject.Count})");
                childNode.Tag = csProject;
                node.Nodes.Add(childNode);

                if (csProject.Count > 0)
                    AddChildren(childNode, csProject);
            }
        }
        else if (item is CsProject csProject)
        {
            foreach (var dependency in csProject.Dependencies)
            {
                var childNode = new TreeNode($"{dependency.PackageName} {dependency.PackageVersion} (versions: {dependency.GetVersions().Count}, usage: {_data.GetUsageCount(dependency.PackageName)})");
                childNode.Tag = dependency;
                node.Nodes.Add(childNode);
            }
        }
        else if (item is ComponentsFolder componentsFolder)
        {
            foreach (var component in componentsFolder.Components)
            {
                var childNode = new TreeNode($"{component.Name} (versions: {component.GetVersions().Count}, usage: {_data.GetUsageCount(component.Name)})");
                childNode.Tag = component;
                node.Nodes.Add(childNode);

                if (component.Count > 0)
                    AddChildren(childNode, component);

                childNode.Text = childNode.Text.Replace("usage: xxx", $"usage: {component.Usage.Count}");
            }
        }
        else if (item is Component component)
        {
            foreach (var dependency in component.GetVersions())
            {
                var childNode = new TreeNode($"{dependency.VersionString}");
                childNode.Tag = dependency;
                node.Nodes.Add(childNode);
            }
        }
        else if (item is FrameworksFolder frameworksFolder)
        {
            foreach (var framework in frameworksFolder.Frameworks)
            {
                var childNode = new TreeNode($"{framework.Name} ({framework.Usage.Count})");
                childNode.Tag = framework;
                node.Nodes.Add(childNode);
            }
        }
        else
        {
            throw new SystemException($"Unknown node type: {item.GetType().Name}");
        }
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
        var n = treeView1.SelectedNode;

        if (n == null)
        {
            listView1.Items.Clear();
            return;
        }

        if (n.Tag is CsProjectsFolder || n.Tag is ComponentsFolder || n.Tag is FrameworksFolder)
        {
            var p = n.Tag as CsProjectsFolder;
            var c = n.Tag as ComponentsFolder;
            var f = n.Tag as FrameworksFolder;

            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Project Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Version", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            var dep = new DependencyInfoList();

            if (p != null)
                dep.AddRange(_data.OrderBy(x => x.ProjectName).ThenBy(x => x.PackageName).ThenBy(x => x.PackageVersion).ToList());
            else if (c != null)
                dep.AddRange(_data.OrderBy(x => x.PackageName).ThenBy(x => x.PackageVersion).ThenBy(x => x.ProjectName).ToList());
            else if (f != null)
                dep.AddRange(_data.OrderBy(x => x.Framework).ThenBy(x => x.ProjectName).ToList());

            foreach (var d in dep)
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString()).Tag = d.ProjectNameCount;
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.GetVersions().Count.ToString()).Tag = d.GetVersions().Count;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else if (n.Tag is CsProject csProject)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Version", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            foreach (var d in csProject.Dependencies.OrderBy(x => x.PackageName).ThenBy(x => x.PackageVersion))
            {
                var li = new ListViewItem(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.GetVersions().Count.ToString()).Tag = d.GetVersions().Count;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else if (n.Tag is DependencyInfo depInfo)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Project Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Version", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            var theSet = _data
                .Where(x => x.PackageName == depInfo.PackageName)
                .OrderBy(x => x.ProjectName)
                .ThenBy(x => x.PackageName)
                .ThenBy(x => x.PackageVersion);

            foreach (var d in theSet)
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString()).Tag = d.ProjectNameCount;
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.GetVersions().Count.ToString()).Tag = d.GetVersions().Count;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else if (n.Tag is Component component)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Project Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Version", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            var theSet = _data
                .Where(x => x.PackageName == component.Name)
                .OrderBy(x => x.ProjectName)
                .ThenBy(x => x.PackageName)
                .ThenBy(x => x.PackageVersion);

            foreach (var d in theSet)
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString()).Tag = d.ProjectNameCount;
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.GetVersions().Count.ToString()).Tag = d.GetVersions().Count;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else if (n.Tag is ComponentVersion version)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Project Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            var theSet = _data
                .Where(x => x.PackageName == version.Name && x.PackageVersion == version.VersionString)
                .OrderBy(x => x.ProjectName)
                .ThenBy(x => x.PackageName)
                .ThenBy(x => x.PackageVersion);

            foreach (var d in theSet)
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString()).Tag = d.ProjectNameCount;
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else if (n.Tag is Framework framework)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView1.Columns.Add("Project Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Name", 190);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Package Version", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Framework", 140);
            listView1.Columns.Add("Count", 50, HorizontalAlignment.Center);

            var theSet = _data
                .Where(x => x.Framework == framework.Name)
                .OrderBy(x => x.Framework)
                .ThenBy(x => x.ProjectName);

            foreach (var d in theSet)
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString()).Tag = d.ProjectNameCount;
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString()).Tag = d.PackageNameCount;
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.GetVersions().Count.ToString()).Tag = d.GetVersions().Count;
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString()).Tag = d.FrameworkCount;
                li.Tag = d;
                listView1.Items.Add(li);
            }

            listView1.EndUpdate();
        }
        else
        {
            throw new SystemException($"Unknown node type: {n.Tag.GetType().Name}");
        }
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