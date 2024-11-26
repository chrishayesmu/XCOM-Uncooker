using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using UnrealArchiveLibrary.Unreal;
using UnrealPackageLibrary;

namespace XCOM_Uncooker_GUI
{
    public partial class Form1 : Form
    {
        private IUnrealArchiveManager? archiveManager;

        private string inputArchivesSourceFolder = "";
        private List<ArchivePath> inputArchivePaths = [];
        private IEnumerable<TextureFileCacheEntry> tfcEntries = [];
        private ArchivePath? lastSelectedArchivePath = null;

        class ArchivePath
        {
            public string FilePath { get; set; } = "";

            public string FileName { get; set; } = "";

            public bool IsChecked = false;

            public FArchive? Archive { get; set; }

            public string DisplayName => FileName + (Archive != null ? $" ({Archive.ExportTable.Count} exports)" : "");
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnFullyLoadArchives_Click(object sender, EventArgs e)
        {
            btnFullyLoadArchives.Enabled = false;
            btnUncookArchives.Enabled = btnFullyLoadArchives.Enabled;

            var selectedArchiveFilePaths = inputArchivePaths.Where(p => p.IsChecked).Select(p => p.FilePath);

            if (archiveManager == null)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });

                archiveManager = new UnrealArchiveManager(loggerFactory);
            }

            Task.Run(() => archiveManager.LoadInputArchives(inputArchivesSourceFolder, selectedArchiveFilePaths, OnProgressEvent, DependencyLoadingMode.All));
        }

        private void btnOpenSourceFolder_Click(object sender, EventArgs e)
        {
            if (dlgOpenSourceFolder.InitialDirectory == "")
            {
                dlgOpenSourceFolder.InitialDirectory = GetLikelyXComDirectory();
            }

            if (dlgOpenSourceFolder.ShowDialog() == DialogResult.OK)
            {
                var path = dlgOpenSourceFolder.SelectedPath;
                txtSourcePath.Text = path;
                inputArchivesSourceFolder = path;

                inputArchivePaths.Clear();
                archiveManager?.Dispose();
                archiveManager = null;

                btnFullyLoadArchives.Enabled = false;
                btnUncookArchives.Enabled = false;

                var filePaths = new List<string>();

                // Look for archive files
                if (Directory.Exists(path))
                {
                    var dirFiles = Directory.GetFiles(inputArchivesSourceFolder, "*.upk", SearchOption.AllDirectories);
                    filePaths.AddRange(dirFiles);

                    tfcEntries = Directory.GetFiles(inputArchivesSourceFolder, "*.tfc", SearchOption.AllDirectories)
                                          .Select(f => new TextureFileCacheEntry() { FilePath = f, TextureFileName = Path.GetFileNameWithoutExtension(f) });
                }

                if (filePaths.Count == 0)
                {
                    btnSelectAllInputArchives.Enabled = false;
                    btnSelectNoInputArchives.Enabled = false;
                    return;
                }

                btnSelectAllInputArchives.Enabled = true;
                btnSelectNoInputArchives.Enabled = true;

                inputArchivePaths = filePaths.Select(path => new ArchivePath()
                {
                    FilePath = path,
                    FileName = Path.GetFileName(path),
                    Archive = null
                }).ToList();

                inputArchivePaths.Sort((a, b) => a.FileName.CompareTo(b.FileName));

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

        private void btnSelectAllInputArchives_Click(object sender, EventArgs e)
        {
            lstInputArchives.BeginUpdate();

            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                lstInputArchives.SetItemChecked(i, true);
                (lstInputArchives.Items[i] as ArchivePath)!.IsChecked = true;
            }

            lstInputArchives.EndUpdate();

            btnFullyLoadArchives.Enabled = ShouldEnableFullyLoadArchivesButton();
            btnUncookArchives.Enabled = btnFullyLoadArchives.Enabled;
        }

        private void btnSelectNoInputArchives_Click(object sender, EventArgs e)
        {
            lstInputArchives.BeginUpdate();

            for (int i = 0; i < lstInputArchives.Items.Count; i++)
            {
                lstInputArchives.SetItemChecked(i, false);
                (lstInputArchives.Items[i] as ArchivePath)!.IsChecked = false;
            }

            lstInputArchives.EndUpdate();

            btnFullyLoadArchives.Enabled = ShouldEnableFullyLoadArchivesButton();
            btnUncookArchives.Enabled = btnFullyLoadArchives.Enabled;
        }

        private void btnUncookArchives_Click(object sender, EventArgs e)
        {
            var uncookForm = new UncookForm(archiveManager!);

            if (uncookForm.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    var outputLinker = archiveManager.UncookArchives(tfcEntries, OnProgressEvent, outputArchivesOverride: uncookForm.SelectedOutputArchives);

                    foreach (var archive in outputLinker.Archives)
                    {
                        PackageOrganizer.TryMatchPackageToFolders(archive, out string folderPath);
                        folderPath = Path.Combine(uncookForm.OutputDirectory, folderPath);

                        archiveManager.WriteArchiveToDisk(archive, folderPath);
                    }
                });
                
            }
        }

        private void lstInputArchives_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var archivePath = lstInputArchives.Items[e.Index] as ArchivePath;
            archivePath!.IsChecked = e.NewValue == CheckState.Checked;
        }

        private void lstInputArchives_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnFullyLoadArchives.Enabled = ShouldEnableFullyLoadArchivesButton();
            btnUncookArchives.Enabled = btnFullyLoadArchives.Enabled;

            // Look up the corresponding archive in the input linker, and if loaded,
            // display its objects in the details panel

            if (lstInputArchives.SelectedItem is not ArchivePath archivePath)
            {
                return;
            }

            // If the selected archive hasn't been loaded yet, empty the panel
            if (archivePath.Archive == null)
            {
                lastSelectedArchivePath = null;
                treeViewExportObjects.Nodes.Clear();
                return;
            }

            // Don't redo the tree if the selection hasn't actually changed. Partly for performance,
            // but also so we don't reset all of the nodes which the user has opened unnecessarily.
            // This check and subsequent assignment are deliberately after the Archive == null check,
            // because we do want to redo the tree when we switch from unloaded to loaded state.
            if (archivePath == lastSelectedArchivePath)
            {
                return;
            }

            lastSelectedArchivePath = archivePath;

            treeViewExportObjects.BeginUpdate();
            treeViewExportObjects.Nodes.Clear();

            foreach (var exportObject in archivePath.Archive.ExportedObjects)
            {
                // Look for top-level objects
                if (exportObject.Outer != null)
                {
                    continue;
                }

                var topLevelNode = new TreeNode(FormatUObjectAsShortText(exportObject, useFullPath: true, includeExportIndex: true));

                AddInnerObjectsToTreeNode(exportObject, topLevelNode);

                treeViewExportObjects.Nodes.Add(topLevelNode);
            }

            treeViewExportObjects.EndUpdate();
        }

        private void txtInputArchivesFilter_TextChanged(object sender, EventArgs e)
        {
            var matchingArchivePaths = inputArchivePaths.Where(archivePath => archivePath.FileName.Contains(txtInputArchivesFilter.Text, StringComparison.OrdinalIgnoreCase)).ToArray();

            lstInputArchives.BeginUpdate();

            lstInputArchives.Items.Clear();
            lstInputArchives.Items.AddRange(matchingArchivePaths);

            for (int i = 0; i < matchingArchivePaths.Length; i++)
            {
                lstInputArchives.SetItemChecked(i, matchingArchivePaths[i].IsChecked);
            }

            lstInputArchives.EndUpdate();
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

        private string GetLikelyXComDirectory()
        {
            string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 200510";

            using (var key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                string? value = key?.GetValue("InstallLocation") as string;

                if (value == null)
                {
                    return "";
                }

                string xewPath = Path.Combine(value, "XEW", "XComGame", "CookedPCConsole");

                return Directory.Exists(xewPath) ? xewPath : "";
            }
        }

        private void OnProgressEvent(ProgressEvent e, int numCompleted, int numTotal)
        {
            string statusLabel, statusText;
            int progressValue;

            statusLabel = e switch
            {
                ProgressEvent.ArchiveHeaderLoaded => "Reading archive headers",
                ProgressEvent.ArchiveBodyLoaded => "Deserializing archive bodies",
                ProgressEvent.DependencyLoaded => "Loading dependencies",
                ProgressEvent.LoadComplete => "Archive load complete",
                ProgressEvent.ArchiveUncookedInMemory => "Uncooking archives in memory",
                ProgressEvent.ArchivePostUncookFixup => "Fixing up uncooked archives",
                ProgressEvent.UncookComplete => "Archive uncooking complete",
                _ => $"UNLABELED EVENT {e}",
            };

            if (e == ProgressEvent.LoadComplete)
            {
                statusText = $"{statusLabel} ({numCompleted} new, {numTotal} total)";
                progressValue = 100;
            }
            else if (e == ProgressEvent.UncookComplete)
            {
                statusText = $"{statusLabel} ({numTotal} archive files generated)";
                progressValue = 100;
            }
            else
            {
                statusText = $"{statusLabel} ({numCompleted}/{numTotal})";
                progressValue = (int) (100 * ((float) numCompleted / numTotal));
            }

            if (e == ProgressEvent.DependencyLoaded)
            {
                statusText = $"{statusLabel} ({numCompleted}/{numTotal}?)";
            }

            // Make sure UI updates are happening on the UI thread
            BeginInvoke(() =>
            {
                toolStripStatusLabel.Text = statusText;
                toolStripProgressBar.Value = progressValue;
                statusStrip.Update();

                if (e == ProgressEvent.LoadComplete)
                {
                    // Loading our archives may have loaded some dependencies also, so we just go through and check all of them
                    // to see if they've been loaded or not
                    foreach (var pathObj in inputArchivePaths)
                    {
                        if (archiveManager.InputLinker.TryGetArchiveWithFileName(Path.GetFileNameWithoutExtension(pathObj.FileName), out var archive))
                        {
                            pathObj.Archive = archive;
                        }
                    }

                    lstInputArchives.Refresh();

                    // Update the export objects pane if the currently selected package just got loaded
                    lstInputArchives_SelectedIndexChanged(null, null);

                    btnFullyLoadArchives.Enabled = ShouldEnableFullyLoadArchivesButton();
                    btnUncookArchives.Enabled = btnFullyLoadArchives.Enabled;
                }
            });
        }

        private bool ShouldEnableFullyLoadArchivesButton()
        {
            return inputArchivePaths.Any(p => p.IsChecked);
        }
    }
}
