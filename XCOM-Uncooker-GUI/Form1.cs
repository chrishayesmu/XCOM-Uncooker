using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using UnrealArchiveLibrary.Unreal;
using UnrealPackageLibrary;

namespace XCOM_Uncooker_GUI
{
    public partial class Form1 : Form
    {
        private IUnrealArchiveManager? archiveManager;

        private string inputArchivesSourceFolder = "";
        private List<ArchivePath> inputArchivePaths = [];

        class ArchivePath
        {
            public string FilePath { get; set; } = "";

            public string FileName { get; set; } = "";

            public FArchive? Archive { get; set; }

            public string DisplayName => FileName + (Archive != null ? $" ({Archive.ExportTable.Count} exports)" : "");
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenSourceFolder_Click(object sender, EventArgs e)
        {
            if (dlgOpenSourceFolder.ShowDialog() == DialogResult.OK)
            {
                var path = dlgOpenSourceFolder.SelectedPath;
                txtSourcePath.Text = path;
                inputArchivesSourceFolder = path;

                inputArchivePaths.Clear();
                btnFullyLoadArchives.Enabled = false;

                var filePaths = new List<string>();

                // Look for archive files
                if (Directory.Exists(path))
                {
                    var dirFiles = Directory.GetFiles(inputArchivesSourceFolder, "*.upk", SearchOption.AllDirectories);

                    filePaths.AddRange(dirFiles);
                }

                if (filePaths.Count == 0)
                {
                    btnSelectAllInputArchives.Enabled = false;
                    btnSelectNoInputArchives.Enabled = false;
                    return;
                }

                btnSelectAllInputArchives.Enabled = true;
                btnSelectNoInputArchives.Enabled = true;

                inputArchivePaths = filePaths.Select(path => new ArchivePath() { 
                    FilePath = path,
                    FileName = Path.GetFileName(path),
                    Archive = null
                }).ToList();

                inputArchivePaths.Sort( (a, b) => a.FileName.CompareTo(b.FileName));

                lstInputArchives.Items.Clear();
                lstInputArchives.BeginUpdate();
                lstInputArchives.DisplayMember = "DisplayName";

                foreach (var pathObj in inputArchivePaths)
                {
                    lstInputArchives.Items.Add(pathObj);
                }

                lstInputArchives.EndUpdate();
            }
        }

        private void btnFullyLoadArchives_Click(object sender, EventArgs e)
        {
            // TODO: it'd be better to retain any already-loaded archives if we can handle it
            archiveManager?.Dispose();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var selectedArchivePaths = new List<ArchivePath>();

            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                if (lstInputArchives.GetItemChecked(i))
                {
                    selectedArchivePaths.Add(inputArchivePaths[i]);
                }
            }

            archiveManager = new UnrealArchiveManager(loggerFactory);
            archiveManager.LoadInputArchives(inputArchivesSourceFolder, selectedArchivePaths.Select(p => p.FilePath), DependencyLoadingMode.All);

            foreach (var pathObj in selectedArchivePaths)
            {
                if (archiveManager.InputLinker.TryGetArchiveWithFileName(Path.GetFileNameWithoutExtension(pathObj.FileName), out var archive))
                {
                    pathObj.Archive = archive;
                }
            }

            lstInputArchives.Refresh();

            /*
            lstInputArchives.Items.Clear();
            lstInputArchives.BeginUpdate();

            // TODO keep all archives in the list, just replace the ones we've loaded
            foreach (var archive in archiveManager.InputLinker.Archives)
            {
                lstInputArchives.Items.Add($"{archive.FileName} ({archive.ExportTable.Count} exports)");
            }

            lstInputArchives.EndUpdate();
            */
        }

        private void btnSelectAllInputArchives_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                lstInputArchives.SetItemChecked(i, true);
            }
        }

        private void btnSelectNoInputArchives_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                lstInputArchives.SetItemChecked(i, false);
            }
        }

        private void lstInputArchives_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool anySelected = false;

            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                if (lstInputArchives.GetItemChecked(i))
                {
                    anySelected = true;
                    break;
                }
            }

            btnFullyLoadArchives.Enabled = anySelected;

            // TODO look up the corresponding archive in the input linker, and if loaded,
            // display its objects in the details panel
            var archivePath = lstInputArchives.SelectedItem as ArchivePath;

            if (archivePath == null)
            {
                return;
            }

            treeViewExportObjects.BeginUpdate();
            treeViewExportObjects.Nodes.Clear();

            if (archivePath.Archive == null)
            {
                treeViewExportObjects.EndUpdate();
                return;
            }

            foreach (var exportObject in archivePath.Archive.ExportedObjects)
            {
                // Look for top-level objects
                if (exportObject.Outer != null && !exportObject.Outer.ExportTableEntry!.IsPackage)
                {
                    continue;
                }

                var topLevelNode = new TreeNode(FormatUObjectAsShortText(exportObject, useFullPath: true, includeExportIndex: true));

                AddInnerObjectsToTreeNode(exportObject, topLevelNode);

                treeViewExportObjects.Nodes.Add(topLevelNode);
            }
            
            treeViewExportObjects.EndUpdate();
        }

        private void AddInnerObjectsToTreeNode(UObject obj, TreeNode node)
        {
            foreach (var innerObj in obj.InnerObjects)
            {
                var newNode = new TreeNode(FormatUObjectAsShortText(innerObj));

                AddInnerObjectsToTreeNode(innerObj, newNode);

                node.Nodes.Add(newNode);
            }
        }

        private string FormatUObjectAsShortText(UObject obj, bool useFullPath = false, bool includeExportIndex = false)
        {
            string text = obj.TableEntry.ClassNameString + " " + (useFullPath ? obj.FullObjectPath : obj.ObjectName);

            if (includeExportIndex)
            {
                text = $"Export {obj.ExportTableEntry!.TableEntryIndex + 1}: {text}";
            }

            return text;
        }
    }
}
