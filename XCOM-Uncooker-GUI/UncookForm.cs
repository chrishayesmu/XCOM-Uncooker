using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnrealPackageLibrary;

namespace XCOM_Uncooker_GUI
{
    public partial class UncookForm : Form
    {
        // TODO validate this list
        public static readonly List<string> NeverUncook = [ "Core", "Engine", "EngineDebugMaterials", "EngineFonts", "EngineMaterials", "EngineMeshes", "EngineResources", "EngineSounds", "EngineVolumetrics", "Engine_MaterialFunctions02", "Engine_MI_Shaders", "GameFramework", "GFxUI", "GFxUIEditor", "IpDrv", "OnlineSubsystemSteamworks", "XComGame", "XComStrategyGame", "XComUIShell" ];

        public ISet<string> SelectedOutputArchives = new HashSet<string>();

        public string OutputDirectory = "";

        private readonly IUnrealArchiveManager archiveManager;

        public UncookForm(IUnrealArchiveManager archiveManager)
        {
            this.archiveManager = archiveManager;

            InitializeComponent();

            lstSourceArchives.BeginUpdate();

            foreach (var archive in archiveManager.InputLinker.Archives)
            {
                lstSourceArchives.Items.Add(archive.NormalizedName);
            }

            lstSourceArchives.EndUpdate();

            lstOutputArchivesUnselected.BeginUpdate();

            var uncookedArchives = new HashSet<string>();

            foreach (var archive in archiveManager.InputLinker.Archives)
            {
                uncookedArchives.UnionWith(archive.TopLevelPackages.Select(p => p.NormalizedName));
            }

            uncookedArchives.ExceptWith(NeverUncook);

            lstOutputArchivesUnselected.Items.AddRange(uncookedArchives.ToArray());

            lstOutputArchivesUnselected.EndUpdate();
        }

        private void btnSelectOutputArchives_Click(object sender, EventArgs e)
        {
            var selectedArchives = new List<object?>();

            foreach (var item in lstOutputArchivesUnselected.SelectedItems)
            {
                selectedArchives.Add(item);
            }

            lstOutputArchivesUnselected.BeginUpdate();
            lstOutputArchivesSelected.BeginUpdate();

            foreach (var archive in selectedArchives)
            {
                if (archive != null)
                {
                    lstOutputArchivesUnselected.Items.Remove(archive);
                    lstOutputArchivesSelected.Items.Add(archive);
                }
            }

            lstOutputArchivesUnselected.EndUpdate();
            lstOutputArchivesSelected.EndUpdate();
        }

        private void btnDeselectOutputArchives_Click(object sender, EventArgs e)
        {
            var selectedArchives = new List<object?>();

            foreach (var item in lstOutputArchivesSelected.SelectedItems)
            {
                selectedArchives.Add(item);
            }

            lstOutputArchivesUnselected.BeginUpdate();
            lstOutputArchivesSelected.BeginUpdate();

            foreach (var archive in selectedArchives)
            {
                if (archive != null)
                {
                    lstOutputArchivesUnselected.Items.Add(archive);
                    lstOutputArchivesSelected.Items.Remove(archive);
                }
            }

            lstOutputArchivesUnselected.EndUpdate();
            lstOutputArchivesSelected.EndUpdate();
        }

        private void btnStartUncookingProcess_Click(object sender, EventArgs e)
        {
            if (dlgChooseUncookDestination.ShowDialog() == DialogResult.OK)
            {
                OutputDirectory = dlgChooseUncookDestination.SelectedPath;

                // Set list of selected archives for the parent form to read
                foreach (var archive in lstOutputArchivesSelected.Items)
                {
                    SelectedOutputArchives.Add((string) archive);
                }
                
                DialogResult = DialogResult.OK;
            }
        }
    }
}
