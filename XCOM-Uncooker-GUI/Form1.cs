using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using System.Diagnostics;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.SerializedProperties;
using UnrealPackageLibrary;

namespace XCOM_Uncooker_GUI
{
    public partial class Form1 : Form
    {
        // These archives will cause problems in the UDK if our uncooked version is loaded, so we
        // don't output them at all to avoid that.
        private static readonly List<string> ArchivesToNeverUncook = [
            "Core",
            "Engine",
            "EngineDebugMaterials",
            "EngineFonts",
            "EngineMaterials",
            "EngineMeshes",
            "EngineResources",
            "EngineSounds",
            "EngineVolumetrics",
            "Engine_MaterialFunctions02",
            "Engine_MI_Shaders",
            "GameFramework",
            "GFxUI",
            "GFxUIEditor",
            "IpDrv",
            "OnlineSubsystemSteamworks",
            "XComGame",
            "XComStrategyGame",
            "XComUIShell"
        ];

        // Classes which we want deserialized from the WotC SDK's content files.
        private static readonly ISet<string> WotcClassesToDeserialize = new HashSet<string>() { 
            "Material",
            "MaterialExpression",
            "MaterialExpressionAbs",
            "MaterialExpressionAdd",
            "MaterialExpressionAntialiasedTextureMask",
            "MaterialExpressionAppendVector",
            "MaterialExpressionBumpOffset",
            "MaterialExpressionCameraVector",
            "MaterialExpressionCameraWorldPosition",
            "MaterialExpressionCeil",
            "MaterialExpressionClamp",
            "MaterialExpressionComment",
            "MaterialExpressionComponentMask",
            "MaterialExpressionConstant",
            "MaterialExpressionConstant2Vector",
            "MaterialExpressionConstant3Vector",
            "MaterialExpressionConstant4Vector",
            "MaterialExpressionConstantBiasScale",
            "MaterialExpressionConstantClamp",
            "MaterialExpressionConstantScale",
            "MaterialExpressionCosine",
            "MaterialExpressionCrossProduct",
            "MaterialExpressionCustom",
            "MaterialExpressionCustomTexture",
            "MaterialExpressionDecalAttenuation",
            "MaterialExpressionDepthBiasBlend",
            "MaterialExpressionDepthBiasedAlpha",
            "MaterialExpressionDepthBiasedBlend",
            "MaterialExpressionDepthOfFieldFunction",
            "MaterialExpressionDeriveNormalZ",
            "MaterialExpressionDesaturation",
            "MaterialExpressionDestColor",
            "MaterialExpressionDestDepth",
            "MaterialExpressionDestWorldPosition",
            "MaterialExpressionDistance",
            "MaterialExpressionDivide",
            "MaterialExpressionDotProduct",
            "MaterialExpressionDynamicParameter",
            "MaterialExpressionDynamicSwitchParameter",
            "MaterialExpressionFlipBookSample",
            "MaterialExpressionFloor",
            "MaterialExpressionFluidNormal",
            "MaterialExpressionFmod",
            "MaterialExpressionFontSample",
            "MaterialExpressionFontSampleParameter",
            "MaterialExpressionFrac",
            "MaterialExpressionFresnel",
            "MaterialExpressionFunctionInput",
            "MaterialExpressionFunctionOutput",
            "MaterialExpressionGammaCorrection",
            "MaterialExpressionGate",
            "MaterialExpressionIf",
            "MaterialExpressionLength",
            "MaterialExpressionLensFlareIntensity",
            "MaterialExpressionLensFlareOcclusion",
            "MaterialExpressionLensFlareRadialDistance",
            "MaterialExpressionLensFlareRayDistance",
            "MaterialExpressionLensFlareSourceDistance",
            "MaterialExpressionLightmapUVs",
            "MaterialExpressionLightmassReplace",
            "MaterialExpressionLightVector",
            "MaterialExpressionLinearInterpolate",
            "MaterialExpressionMaterialFunctionCall",
            "MaterialExpressionMax",
            "MaterialExpressionMeshEmitterDynamicParameter",
            "MaterialExpressionMeshEmitterVertexColor",
            "MaterialExpressionMeshSubUV",
            "MaterialExpressionMeshSubUVBlend",
            "MaterialExpressionMin",
            "MaterialExpressionMultiply",
            "MaterialExpressionNormalize",
            "MaterialExpressionObjectOrientation",
            "MaterialExpressionObjectRadius",
            "MaterialExpressionObjectWorldPosition",
            "MaterialExpressionOcclusionPercentage",
            "MaterialExpressionOneMinus",
            "MaterialExpressionPanner",
            "MaterialExpressionParameter",
            "MaterialExpressionParticleMacroUV",
            "MaterialExpressionParticleSubUV",
            "MaterialExpressionPerInstanceRandom",
            "MaterialExpressionPixelDepth",
            "MaterialExpressionPower",
            "MaterialExpressionReflect",
            "MaterialExpressionReflectionVector",
            "MaterialExpressionRotateAboutAxis",
            "MaterialExpressionRotator",
            "MaterialExpressionScalarParameter",
            "MaterialExpressionSceneCaptureState",
            "MaterialExpressionSceneDepth",
            "MaterialExpressionSceneTexture",
            "MaterialExpressionScreenPosition",
            "MaterialExpressionScreenTexelSize",
            "MaterialExpressionSine",
            "MaterialExpressionSmoothstep",
            "MaterialExpressionSphereMask",
            "MaterialExpressionSplice",
            "MaterialExpressionSplit",
            "MaterialExpressionSquareRoot",
            "MaterialExpressionStaticBool",
            "MaterialExpressionStaticBoolParameter",
            "MaterialExpressionStaticComponentMaskParameter",
            "MaterialExpressionStaticSwitch",
            "MaterialExpressionStaticSwitchParameter",
            "MaterialExpressionSubtract",
            "MaterialExpressionTerrainLayerCoords",
            "MaterialExpressionTerrainLayerSwitch",
            "MaterialExpressionTerrainLayerWeight",
            "MaterialExpressionTextureCoordinate",
            "MaterialExpressionTextureObject",
            "MaterialExpressionTextureObjectParameter",
            "MaterialExpressionTextureSample",
            "MaterialExpressionTextureSampleParameter",
            "MaterialExpressionTextureSampleParameter2D",
            "MaterialExpressionTextureSampleParameterCube",
            "MaterialExpressionTextureSampleParameterFlipbook",
            "MaterialExpressionTextureSampleParameterMeshSubUV",
            "MaterialExpressionTextureSampleParameterMeshSubUVBlend",
            "MaterialExpressionTextureSampleParameterMovie",
            "MaterialExpressionTextureSampleParameterNormal",
            "MaterialExpressionTextureSampleParameterSubUV",
            "MaterialExpressionTime",
            "MaterialExpressionTransform",
            "MaterialExpressionTransformPosition",
            "MaterialExpressionTwoSidedSign",
            "MaterialExpressionVectorParameter",
            "MaterialExpressionVertexColor",
            "MaterialExpressionVertexTextureSampleParameter2D",
            "MaterialExpressionViewportPosition",
            "MaterialExpressionWindDirectionAndSpeed",
            "MaterialExpressionWorldNormal",
            "MaterialExpressionWorldPosition",
            "MaterialExpressionXComCursorPosition",
            "MaterialExpressionXComDynamicSwitchParameter",
            "MaterialExpressionXComFOWVolume",
            "MaterialExpressionXComVectorParameter",
            "MaterialFunction"
        };

        // Expression property names which we should copy data from WotC for.
        private static readonly string[] WotcMaterialExpressionProperties = [
            "DiffuseColor",
            "DiffusePower",
            "SpecularColor",
            "SpecularPower",
            "Normal",
            "EmissiveColor",
            "Opacity",
            "OpacityMask",
            "CustomLighting",
            "CustomSkylightDiffuse",
            "AnisotropicDirection",
            "TwoSidedLightingMask",
            "TwoSidedLightingColor",
            "WorldPositionOffset",
            "WorldDisplacement",
            "TessellationMultiplier",
            "SubsurfaceInscatteringColor",
            "SubsurfaceAbsorptionColor",
            "SubsurfaceScatteringRadius"
        ];

        // These are the specific directories we're interested in loading from the WotC SDK. They are
        // relative to the XComGame/Content directory within the SDK's root.
        private static readonly string[] WotcSdkContentDirectories = [
            "XCOM_2\\Packages\\FX\\_FX_Legacy\\FX_EU", 
            "XCOM_2\\Packages\\FX\\_FX_Legacy\\FX_EW"
        ];

        // These are the uncooked script files we want to load from the WotC SDK.
        private static readonly string[] WotcSdkScriptPackages = [
            "Core.u",
            "Engine.u",
            "XComEditor.u",
            "XComGame.u"
        ];

        private IUnrealArchiveManager? xcomEwArchiveManager;
        private IUnrealArchiveManager? xcomWotcSdkArchiveManager;
        private Stopwatch stopwatch = new Stopwatch();

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

            timerEachSecond.Tick += UpdateTimerText;
        }

        private void UpdateTimerText(object? sender, EventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                TimeSpan elapsedTime = stopwatch.Elapsed;
                toolStripTimer.Text = $"({elapsedTime.ToString("mm\\:ss")} elapsed)";
            }
        }

        private void btnFullyLoadArchives_Click(object sender, EventArgs e)
        {
            btnFullyLoadArchives.Enabled = false;
            btnUncookArchives.Enabled = false;

            var selectedArchiveFilePaths = inputArchivePaths.Where(p => p.IsChecked).Select(p => p.FilePath);

            if (xcomEwArchiveManager == null)
            {
                xcomEwArchiveManager = new UnrealArchiveManager(CreateLoggerFactory());
            }

            stopwatch.Restart();

            Task.Run(() => xcomEwArchiveManager.LoadInputArchives(inputArchivesSourceFolder, selectedArchiveFilePaths, progressHandler: OnProgressEvent, dependencyMode: DependencyLoadingMode.All));
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
                xcomEwArchiveManager?.Dispose();
                xcomEwArchiveManager = null;

                btnFullyLoadArchives.Enabled = false;
                btnUncookArchives.Enabled = false;
                btnOpenSourceFolder.Enabled = false;
                btnOpenWotcSdkPath.Enabled = false;

                var filePaths = new List<string>();

                // Look for archive files
                if (Directory.Exists(path))
                {
                    var extensions = new HashSet<string>
                    {
                        ".u",
                        ".udk",
                        ".upk"
                    };

                    var dirFiles = Directory.EnumerateFiles(inputArchivesSourceFolder, "*.*", SearchOption.AllDirectories)
                                            .Where(p => extensions.Contains(Path.GetExtension(p)));

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

                // Don't allow archives starting with "patch_" because Long War ships some map patches in this way,
                // and the archive library doesn't understand that they don't use the XCOM-specific data formats yet
                inputArchivePaths = filePaths
                    .Where(path => !Path.GetFileName(path).StartsWith("patch_"))
                    .Select(path => new ArchivePath()
                    {
                        FilePath = path,
                        FileName = Path.GetFileName(path),
                        Archive = null
                    })
                    .ToList();

                inputArchivePaths.Sort((a, b) => a.FileName.CompareTo(b.FileName));

                lstInputArchives.Items.Clear();
                lstInputArchives.BeginUpdate();
                lstInputArchives.DisplayMember = "DisplayName";

                foreach (var pathObj in inputArchivePaths)
                {
                    lstInputArchives.Items.Add(pathObj);
                }

                lstInputArchives.EndUpdate();

                btnOpenSourceFolder.Enabled = true;
                btnOpenWotcSdkPath.Enabled = true;
            }
        }

        private void btnOpenWotcSdkPath_Click(object sender, EventArgs e)
        {
            if (dlgOpenWotcSdkFolder.InitialDirectory == "")
            {
                dlgOpenWotcSdkFolder.InitialDirectory = GetLikelyWotcSdkDirectory();
            }

            if (dlgOpenWotcSdkFolder.ShowDialog() == DialogResult.OK)
            {
                string wotcSdkBaseDir = dlgOpenWotcSdkFolder.SelectedPath;
                txtWotcSdkPath.Text = wotcSdkBaseDir;

                btnOpenSourceFolder.Enabled = false;
                btnOpenWotcSdkPath.Enabled = false;

                xcomWotcSdkArchiveManager?.Dispose();
                xcomWotcSdkArchiveManager = new UnrealArchiveManager(CreateLoggerFactory());

                Task.Run(() =>
                {
                    // First load the uncooked script packages
                    string scriptDir = Path.Combine(wotcSdkBaseDir, "XComGame", "Script");
                    var scriptFiles = WotcSdkScriptPackages.Select(f => Path.Combine(scriptDir, f));
                    xcomWotcSdkArchiveManager.LoadInputArchives(scriptDir, scriptFiles, progressHandler: OnProgressEvent, dependencyMode: DependencyLoadingMode.All);

                    // Now add the uncooked content packages
                    string contentDir = Path.Combine(wotcSdkBaseDir, "XComGame", "Content");
                    string baseDir = Path.Combine(contentDir, "XCOM_2"); // limit dependency resolution to this folder
                    var contentFiles = WotcSdkContentDirectories.Select(p => Path.Combine(contentDir, p)).SelectMany(p => Directory.GetFiles(p, "*.upk"))
                                   .Where(p => !p.EndsWith("epacePackage.upk"));

                    xcomWotcSdkArchiveManager.LoadInputArchives(baseDir, contentFiles, classWhitelist: WotcClassesToDeserialize, progressHandler: OnProgressEvent, dependencyMode: DependencyLoadingMode.All);

                    BeginInvoke(() =>
                    {
                        btnOpenSourceFolder.Enabled = true;
                        btnOpenWotcSdkPath.Enabled = true;
                    });
                });
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
        }

        private void btnUncookArchives_Click(object sender, EventArgs e)
        {
            if (xcomEwArchiveManager == null)
            {
                return;
            }

            var uncookForm = new UncookForm(xcomEwArchiveManager);

            if (uncookForm.ShowDialog() == DialogResult.OK)
            {
                stopwatch.Restart();

                Task.Run(() =>
                {
                    var outputLinker = xcomEwArchiveManager.UncookArchives(tfcEntries, OnProgressEvent, outputArchivesOverride: uncookForm.SelectedOutputArchives);
                    var archives = outputLinker.Archives.Where(archive => !ArchivesToNeverUncook.Contains(archive.FileName));
                    int numArchivesWritten = 0, totalArchives = archives.Count();

                    var availableTextures = new Dictionary<string, UObject>();

                    foreach (var archive in archives)
                    {
                        foreach (var exportEntry in archive.ExportTable)
                        {
                            if (exportEntry.ClassNameString == "Texture2D")
                            {
                                availableTextures.Add(exportEntry.ObjectName, exportEntry.ExportObject);
                            }
                        }
                    }

                    // TODO move MaxDegreeOfParallelism to an exposed setting
                    Parallel.ForEach(archives, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, archive =>
                    {
                        CopyWotcMaterialExpressionsForArchive(archive, availableTextures);

                        PackageOrganizer.TryMatchPackageToFolders(archive, out string folderPath);
                        folderPath = Path.Combine(uncookForm.OutputDirectory, folderPath);

                        xcomEwArchiveManager.WriteArchiveToDisk(archive, folderPath);

                        OnProgressEvent(ProgressEvent.ArchiveWrittenToDisk, Interlocked.Increment(ref numArchivesWritten), totalArchives);
                    });
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

        private void CopyWotcMaterialExpressionsForArchive(FArchive archive, IDictionary<string, UObject> availableTextures)
        {
            if (xcomWotcSdkArchiveManager == null)
            {
                return;
            }

            var matchingWotcArchive = xcomWotcSdkArchiveManager.InputLinker.Archives.Find(ar => ar.NormalizedName == archive.NormalizedName);

            if (matchingWotcArchive == null)
            {
                Console.WriteLine("");
                return;
            }

            // Cloning new objects may modify the archive's ExportedObjects; we only want to iterate the
            // ones which are already part of the archive
            int originalExportCount = archive.ExportedObjects.Count;

            for (int i = 0; i < originalExportCount; i++)
            {
                var exportObj = archive.ExportedObjects[i];

                if (exportObj == null)
                {
                    continue;
                }

                if (exportObj.TableEntry.ClassNameString != "Material")
                {
                    continue;
                }

                var matchingExportObj = matchingWotcArchive.ExportedObjects.FirstOrDefault(obj => obj?.FullObjectPath == exportObj.FullObjectPath);

                if (matchingExportObj == null)
                {
                    continue;
                }

                // If we're able to find the same material from the WotC SDK, copy the material expressions over
                // TODO update the Expressions array prop
                // TODO remove the material fixup logic from within the archive library
                foreach (string expressionPropName in WotcMaterialExpressionProperties)
                {
                    var wotcProp = matchingExportObj.GetSerializedProperty(expressionPropName);

                    if (wotcProp == null)
                    {
                        continue;
                    }

                    // Remove the existing expression property if there is one
                    var ewProp = exportObj.GetSerializedProperty(expressionPropName);

                    if (ewProp != null)
                    {
                        exportObj.SerializedProperties.Remove(ewProp);
                    }

                    var clonedProp = wotcProp.CloneToOtherArchive(archive);
                    exportObj.SerializedProperties.Add(clonedProp);
                }
            }

            // Now that we've cloned a bunch of properties, the expressions they're referencing will have export table
            // entries, but won't be present in ExportedObjects. We just need to go through and copy those objects into
            // our archive also.
            for (int i = originalExportCount; i < archive.ExportTable.Count; i++)
            {
                if (archive.ExportedObjects[i] != null)
                {
                    continue;
                }

                string objectPath = archive.ExportTable[i].FullObjectPath;
                var wotcObject = matchingWotcArchive.GetExportedObjectByPath(objectPath, archive.ExportTable[i]);

                if (wotcObject == null)
                {
                    Console.WriteLine("impossible??");
                    continue;
                }

                var newObject = archive.AddExportObject(wotcObject);

                if (newObject == null)
                {
                    throw new Exception($"Failed to copy export object {wotcObject.FullObjectPath} from WotC SDK");
                }

                // Texture sample expressions in the WotC SDK will be referencing textures based on their locations in
                // that SDK, but some of them have been moved since EW was released. We need to check if the texture's in
                // the right spot, and redirect it if not.
                if (newObject.TableEntry.ClassNameString == "MaterialExpressionTextureSample")
                {
                    var textureProp = newObject.GetSerializedProperty("Texture") as USerializedObjectProperty;

                    // Only interested in imports for now; presumably an export wouldn't have this problem
                    if (textureProp != null && textureProp.ObjectIndex < 0)
                    {
                        var textureObject = archive.GetObjectByIndex(textureProp.ObjectIndex);

                        if (textureObject == null)
                        {
                            var existingTableEntry = archive.GetImportTableEntry(textureProp.ObjectIndex);

                            if (availableTextures.TryGetValue(existingTableEntry.ObjectName, out UObject newTextureObj))
                            {
                                int packageImportIndex = archive.GetOrCreateImport(archive.GetOrCreateName("Core"), archive.GetOrCreateName("Package"), archive.GetOrCreateName(newTextureObj.Archive.NormalizedName), 0);
                                int textureImportIndex = archive.GetOrCreateImport(existingTableEntry.ClassPackage, existingTableEntry.ClassName, existingTableEntry.ObjectName, packageImportIndex);

                                textureProp.ObjectIndex = textureImportIndex;
                            }
                        }
                    }
                }
            }

            // TODO: manually break apart the ConstantScale expression into a multiply + constant
            // TODO: try to clean up imports of packages that won't exist? going to leave annoying gaps in the import table, but the imports cause warnings in the UDK
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
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

        private string GetLikelyWotcSdkDirectory()
        {
            string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 602410";

            using (var key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                string? value = key?.GetValue("InstallLocation") as string;

                if (value == null)
                {
                    return "";
                }

                return Directory.Exists(value) ? value : "";
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
                ProgressEvent.ArchiveWrittenToDisk => "Writing archives to disk",
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

            // Stop time measurement if needed
            else if (e == ProgressEvent.LoadComplete)
            {
                stopwatch.Stop();
            }
            else if (e == ProgressEvent.ArchiveWrittenToDisk && progressValue == 100)
            {
                stopwatch.Stop();

                statusText = $"Uncooking complete: {numTotal} archive file(s) written to disk";
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
                    if (xcomEwArchiveManager != null)
                    {
                        foreach (var pathObj in inputArchivePaths)
                        {
                            if (xcomEwArchiveManager.InputLinker.TryGetArchiveWithFileName(Path.GetFileNameWithoutExtension(pathObj.FileName), out var archive))
                            {
                                pathObj.Archive = archive;
                            }
                        }
                    }

                    lstInputArchives.Refresh();

                    // Update the export objects pane if the currently selected package just got loaded
                    lstInputArchives_SelectedIndexChanged(null, null);

                    btnFullyLoadArchives.Enabled = ShouldEnableFullyLoadArchivesButton();
                    btnUncookArchives.Enabled = ShouldEnableUncookButton();
                }
            });
        }

        private bool ShouldEnableFullyLoadArchivesButton()
        {
            return inputArchivePaths.Any(p => p.IsChecked);
        }

        private bool ShouldEnableUncookButton()
        {
            return xcomEwArchiveManager != null && xcomEwArchiveManager.InputLinker.Archives.Count > 0;
        }
    }
}
