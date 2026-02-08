#nullable enable
using PackageDependencyCheckerLibrary;
using PackageDependencyCheckerLibrary.TreeStructure;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PackageDependencies;

public partial class MainWindow : Form
{
    private DependencyInfoList _data;
    public const string ApplicationName = "Package Dependencies";
    
    public MainWindow()
    {
        _data = [];
        InitializeComponent();
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
                var childNode = new TreeNode($"{dependency.PackageName} {dependency.PackageVersion}");
                childNode.Tag = dependency;
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

        if (n.Tag is CsProjectsFolder csProjectsFolder)
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

            foreach (var d in _data.OrderBy(x => x.ProjectName).ThenBy(x => x.PackageName).ThenBy(x => x.PackageVersion))
            {
                var li = new ListViewItem(d.ProjectName);
                li.SubItems.Add(d.ProjectNameCount.ToString());
                li.SubItems.Add(d.PackageName);
                li.SubItems.Add(d.PackageNameCount.ToString());
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.PackageVersionCount.ToString());
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString());
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
                li.SubItems.Add(d.PackageNameCount.ToString());
                li.SubItems.Add(d.PackageVersion);
                li.SubItems.Add(d.PackageVersionCount.ToString());
                li.SubItems.Add(d.Framework);
                li.SubItems.Add(d.FrameworkCount.ToString());
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
}