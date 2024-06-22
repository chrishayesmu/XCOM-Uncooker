using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Engine;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Anims;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Audio;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Flash;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Level;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Sequences;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Textures;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.XCom;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class UObject
    {
        private static readonly Logger Log = new Logger(nameof(UObject));

        private static readonly IDictionary<string, Func<FArchive, FObjectTableEntry, UObject>> ClassNameToUObjectFactory = new Dictionary<string, Func<FArchive, FObjectTableEntry, UObject>>()
        {
            { "Class",             (archive, tableEntry) => new UClass(archive, tableEntry) },
            { "Const",             (archive, tableEntry) => new UConst(archive, tableEntry) },
            { "Enum",              (archive, tableEntry) => new UEnum(archive, tableEntry) },
            { "Function",          (archive, tableEntry) => new UFunction(archive, tableEntry) },
            { "MetaData",          (archive, tableEntry) => new UMetaData(archive, tableEntry) },
            { "ObjectRedirector",  (archive, tableEntry) => new UObjectRedirector(archive, tableEntry) },
            { "Package",           (archive, tableEntry) => new UPackage(archive, tableEntry) },
            { "State",             (archive, tableEntry) => new UState(archive, tableEntry) },
            { "ScriptStruct",      (archive, tableEntry) => new UScriptStruct(archive, tableEntry) },

            { "ArrayProperty",     (archive, tableEntry) => new UArrayProperty(archive, tableEntry) },
            { "BoolProperty",      (archive, tableEntry) => new UBoolProperty(archive, tableEntry) },
            { "ByteProperty",      (archive, tableEntry) => new UByteProperty(archive, tableEntry) },
            { "ClassProperty",     (archive, tableEntry) => new UClassProperty(archive, tableEntry) },
            { "ComponentProperty", (archive, tableEntry) => new UComponentProperty(archive, tableEntry) },
            { "DelegateProperty",  (archive, tableEntry) => new UDelegateProperty(archive, tableEntry) },
            { "FloatProperty",     (archive, tableEntry) => new UFloatProperty(archive, tableEntry) },
            { "IntProperty",       (archive, tableEntry) => new UIntProperty(archive, tableEntry) },
            { "InterfaceProperty", (archive, tableEntry) => new UInterfaceProperty(archive, tableEntry) },
            { "MapProperty",       (archive, tableEntry) => new UMapProperty(archive, tableEntry) },
            { "NameProperty",      (archive, tableEntry) => new UNameProperty(archive, tableEntry) },
            { "ObjectProperty",    (archive, tableEntry) => new UObjectProperty(archive, tableEntry) },
            { "StrProperty",       (archive, tableEntry) => new UStrProperty(archive, tableEntry) },
            { "StructProperty",    (archive, tableEntry) => new UStructProperty(archive, tableEntry) },

            { "AnimSequence",                      (archive, tableEntry) => new UAnimSequence(archive, tableEntry) },
            { "ApexGenericAsset",                  (archive, tableEntry) => new NvApexGenericAsset(archive, tableEntry) },
            { "BrushComponent",                    (archive, tableEntry) => new UBrushComponent(archive, tableEntry) },
            { "DecalComponent",                    (archive, tableEntry) => new UDecalComponent(archive, tableEntry) },
            { "DecalMaterial" ,                    (archive, tableEntry) => new UMaterial(archive, tableEntry) },
            { "DominantDirectionalLightComponent", (archive, tableEntry) => new UDominantDirectionalLightComponent(archive, tableEntry) },
            { "DominantSpotLightComponent",        (archive, tableEntry) => new UDominantSpotLightComponent(archive, tableEntry) },
            { "FaceFXAnimSet",                     (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) },
            { "FaceFXAsset",                       (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) },
            { "FluidSurfaceComponent",             (archive, tableEntry) => new UFluidSurfaceComponent(archive, tableEntry) },
            { "Font",                              (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) },
            { "FracturedStaticMesh"         ,      (archive, tableEntry) => new UFracturedStaticMesh(archive, tableEntry) },
            { "FracturedStaticMeshComponent",      (archive, tableEntry) => new UStaticMeshComponent(archive, tableEntry) },
            { "GuidCache",                         (archive, tableEntry) => new UGuidCache(archive, tableEntry) },
            { "ImageBasedReflectionComponent",     (archive, tableEntry) => new UStaticMeshComponent(archive, tableEntry) },
            { "InstancedStaticMeshComponent",      (archive, tableEntry) => new UInstancedStaticMeshComponent(archive, tableEntry) },
            { "Level",                             (archive, tableEntry) => new ULevel(archive, tableEntry) },
            { "LightMapTexture2D",                 (archive, tableEntry) => new ULightMapTexture2D(archive, tableEntry) },
            { "Material",                          (archive, tableEntry) => new UMaterial(archive, tableEntry) },
            { "MaterialInstance",                  (archive, tableEntry) => new UMaterialInstance(archive, tableEntry) },
            { "MaterialInstanceConstant",          (archive, tableEntry) => new UMaterialInstance(archive, tableEntry) },
            { "MaterialInstanceTimeVarying",       (archive, tableEntry) => new UMaterialInstance(archive, tableEntry) },
            { "Model",                             (archive, tableEntry) => new UModel(archive, tableEntry) },
            { "ModelComponent",                    (archive, tableEntry) => new UModelComponent(archive, tableEntry) },
            { "Polys",                             (archive, tableEntry) => new UPolys(archive, tableEntry) },
            { "PhysicsAssetInstance",              (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) },
            { "PrefabInstance",                    (archive, tableEntry) => new APrefabInstance(archive, tableEntry) },
            { "RB_BodySetup",                      (archive, tableEntry) => new URB_BodySetup(archive, tableEntry) },
            { "SeqAct_Interp",                     (archive, tableEntry) => new USeqAct_Interp(archive, tableEntry) },
            { "Sequence",                          (archive, tableEntry) => new USequence(archive, tableEntry) },
            { "ShadowMap1D",                       (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) },
            { "ShadowMapTexture2D",                (archive, tableEntry) => new UTexture2D(archive, tableEntry) },
            { "SkeletalMesh",                      (archive, tableEntry) => new USkeletalMesh(archive, tableEntry) },
            { "SoundClass",                        (archive, tableEntry) => new USoundClass(archive, tableEntry) },
            { "SoundCue",                          (archive, tableEntry) => new USoundCue(archive, tableEntry) },
            { "SoundNodeWave",                     (archive, tableEntry) => new USoundNodeWave(archive, tableEntry) },
            { "SpeedTreeComponent",                (archive, tableEntry) => new USpeedTreeComponent(archive, tableEntry) },
            { "StaticMesh",                        (archive, tableEntry) => new UStaticMesh(archive, tableEntry) },
            { "StaticMeshComponent",               (archive, tableEntry) => new UStaticMeshComponent(archive, tableEntry) },
            { "SwfMovie",                          (archive, tableEntry) => new USwfMovie(archive, tableEntry) },
            { "Texture2D",                         (archive, tableEntry) => new UTexture2D(archive, tableEntry) },
            { "TextureCube",                       (archive, tableEntry) => new UTexture(archive, tableEntry) },
            { "TextureFlipBook",                   (archive, tableEntry) => new UTexture2D(archive, tableEntry) },
            { "TextureMovie",                      (archive, tableEntry) => new UTextureMovie(archive, tableEntry) },
            { "TextureRenderTarget2D",             (archive, tableEntry) => new UTexture(archive, tableEntry) },
            { "VisGroupActor",                     (archive, tableEntry) => new XVisGroupActor(archive, tableEntry) },
            { "World",                             (archive, tableEntry) => new UWorld(archive, tableEntry) },

            { "XComCharacterVoiceBank",            (archive, tableEntry) => new XComCharacterVoiceBank(archive, tableEntry) },
            { "XComDebrisStaticMeshComponent",     (archive, tableEntry) => new UStaticMeshComponent(archive, tableEntry) },
            { "XComDestructionInstData",           (archive, tableEntry) => new XComDestructionInstData(archive, tableEntry) },
            { "XComFracDebrisComponent",           (archive, tableEntry) => new UInstancedStaticMeshComponent(archive, tableEntry) },
            { "XComFracDecoComponent",             (archive, tableEntry) => new UInstancedStaticMeshComponent(archive, tableEntry) },
            { "XComVisGroupData",                  (archive, tableEntry) => new XComVisGroupData(archive, tableEntry) },
            { "XComWorldData",                     (archive, tableEntry) => new XComWorldData(archive, tableEntry) },
            { "XComWorldDataContainer",            (archive, tableEntry) => new UAppendedBinaryDataObject(archive, tableEntry) }, // no idea what's in here, praying it's somehow portable (probably not)
        };

        // The proper way to check if a class is a UComponent is to traverse its class hierarchy, but since we're dealing with a fixed set of classes for XCOM,
        // it's faster to just hardcode the components in a set instead
        private static readonly ISet<string> KnownComponentSubtypes = new HashSet<string> ()
        {
            "DistributionFloatConstant",  "DistributionFloatConstantCurve",  "DistributionFloatParameterBase",  "DistributionFloatParticleParameter",  "DistributionFloatSoundParameter", "DistributionFloatUniform",  "DistributionFloatUniformCurve",
            "DistributionVectorConstant", "DistributionVectorConstantCurve", "DistributionVectorParameterBase", "DistributionVectorParticleParameter",                                    "DistributionVectorUniform", "DistributionVectorUniformCurve",
            "RB_Handle", "RB_Spring",
            "ScriptSceneView",
            "XComExplosionLight",
            "XComWeaponComponent", "XComWeaponComponent_Grenade", "XComWeaponComponent_Melee", "XComWeaponComponent_Projectile", "XComWeaponComponent_Shotgun",
            "XGUnitFlyingRing",
        };

        public FArchive Archive;
        public FObjectTableEntry TableEntry;

        public IList<UObject> InnerObjects = new List<UObject>();

        public FExportTableEntry? ExportTableEntry => TableEntry as FExportTableEntry;
        public FImportTableEntry? ImportTableEntry => TableEntry as FImportTableEntry;

        public bool IsExport => ExportTableEntry != null;
        public bool IsImport => ImportTableEntry != null;

        /// <summary>
        /// This object's name, as defined in either its import or export table entry.
        /// </summary>
        public FName ObjectName => TableEntry.ObjectName;

        /// <inheritdoc cref="FObjectTableEntry.Outer" />
        public UObject Outer => TableEntry.Outer;

        /// <inheritdoc cref="FObjectTableEntry.OuterTable" />
        public FObjectTableEntry OuterTable => TableEntry.OuterTable;

        /// <inheritdoc cref="FObjectTableEntry.FullObjectPath" />
        public string FullObjectPath => TableEntry.FullObjectPath;

        #region Serialized data

        /// <summary>
        /// Only used for objects with <see cref="ObjectFlag.HasStack"/> flag set.
        /// </summary>
        public FStateFrame StateFrame = new FStateFrame();

        /// <summary>
        /// Only set for Component objects, and only some of those. 
        /// </summary>
        [Index(typeof(UClass))]
        public int TemplateOwnerClass;

        /// <summary>
        /// Only set for Component objects which are owned by a Class Default Object.
        /// </summary>
        public FName TemplateName;

        /// <summary>
        /// Used for serializing objects during netplay.
        /// </summary>
        public int NetIndex;

        public List<USerializedProperty> SerializedProperties = [];

        #endregion

        public UObject(FArchive archive, FObjectTableEntry tableEntry)
        {
            this.Archive = archive;
            this.TableEntry = tableEntry;
            
            if (tableEntry is FExportTableEntry exportTableEntry)
            {
                exportTableEntry.ExportObject = this;
            }
        }

        public virtual void Serialize(IUnrealDataStream stream)
        {
            // For some reason, every XVisGroupActor claims to have a stack and is lying
            if (ExportTableEntry.ObjectFlags.HasFlag(ObjectFlag.HasStack) && this is not XVisGroupActor)
            {
                StateFrame.Serialize(stream);
            }

            if (!IsClassDefaultObject())
            {
                if (IsComponent())
                {
                    SerializeComponentData(stream);
                }
            }

            stream.Int32(ref NetIndex);

            // Classes don't have tagged properties
            if (this is not UClass)
            {
                SerializeScriptProperties(SerializedProperties, stream);
            }
        }

        /// <summary>
        /// Causes this object to update its data from another object, which is assumed to
        /// belong to a different <see cref="FArchive"/>.
        /// </summary>
        /// <param name="sourceObj"></param>
        public virtual void CloneFromOtherArchive(UObject sourceObj)
        {
            StateFrame.CloneFromOtherArchive(sourceObj.StateFrame, sourceObj.Archive, Archive);
            TemplateOwnerClass = Archive.MapIndexFromSourceArchive(sourceObj.TemplateOwnerClass, sourceObj.Archive);
            TemplateName = Archive.MapNameFromSourceArchive(sourceObj.TemplateName);
            NetIndex = sourceObj.NetIndex;

            SerializedProperties = new List<USerializedProperty>(sourceObj.SerializedProperties.Count);

            for (int i = 0; i < sourceObj.SerializedProperties.Count; i++)
            {
                var propCopy = sourceObj.SerializedProperties[i].CloneToOtherArchive(Archive);
                SerializedProperties.Add(propCopy);
            }
        }

        /// <summary>
        /// Called after an entire archive has been cloned. This is called on objects in the newly cloned archive,
        /// not the source archive. Objects can use this if they need to do any post-processing before the archive
        /// is serialized back to disk.
        /// </summary>
        public virtual void PostArchiveCloneComplete()
        {
        }

        /// <summary>
        /// Gets a serialized property associated with this object by its name, if possible. The property may be
        /// missing if it wasn't serialized (e.g. because it didn't differ from its default value), or if the name
        /// is just plain wrong.
        /// </summary>
        public USerializedProperty GetSerializedProperty(string propName)
        {
            foreach (var prop in SerializedProperties)
            {
                if (prop.Tag?.Name == propName)
                {
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if this object's class has the given class name anywhere in its inheritance chain. Note that
        /// this is not scoped by package. If two different classes are defined in separate packages which have the 
        /// same name, either of those classes would satisfy this inheritance check for that name.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public bool IsA(string className)
        {
            UClass clazz = TableEntry.ClassObj;

            while (clazz != null)
            {
                if (clazz.ObjectName == className)
                {
                    return true;
                }

                clazz = clazz.ExportTableEntry.SuperClassObj;
            }

            return false;
        }

        /// <summary>
        /// Whether this object is an Actor component.
        /// </summary>
        public bool IsComponent()
        {
            if (this is UClass || ExportTableEntry.ObjectFlags.HasFlag(ObjectFlag.ClassDefaultObject))
            {
                return false;
            }

            if (KnownComponentSubtypes.Contains(TableEntry.ClassNameString))
            {
                return true;
            }

            // TODO: very rough heuristic
            if (TableEntry.ClassNameString.EndsWith("Component"))
            {
                return true;
            }

            if (IsImport)
            {
                // TODO: without linking imports, we don't have enough data to say for sure whether this is a component
                return false;
            }

            int classIndex = ExportTableEntry!.ClassIndex;
            while (classIndex != 0)
            {
                var classTableEntry = Archive.GetObjectTableEntry(classIndex);

                if (classTableEntry.ClassNameString.EndsWith("Component"))
                {
                    return true;
                }

                classIndex = classTableEntry is FExportTableEntry ? (classTableEntry as FExportTableEntry).SuperIndex : 0;
            }

            return false;
        }

        /// <summary>
        /// Whether this object is a ClassDefaultObject (CDO), which has different serialization rules.
        /// </summary>
        public bool IsClassDefaultObject()
        {
            if (IsExport)
            {
                return ExportTableEntry.ObjectFlags.HasFlag(ObjectFlag.ClassDefaultObject);
            }

            // Heuristic for imports, since we haven't linked to the source archive
            return Archive.NameToString(ImportTableEntry.ObjectName).StartsWith("Default__");
        }

        protected FName[] CloneNameArrayFromArchive(FName[] array)
        {
            FName[] destArray = new FName[array.Length];

            for (int i = 0; i < destArray.Length; i++)
            {
                destArray[i] = Archive.MapNameFromSourceArchive(array[i]);
            }

            return destArray;
        }

        protected void SerializeComponentData(IUnrealDataStream stream)
        {
            stream.Int32(ref TemplateOwnerClass);

            if (IsOwnedByClassDefaultObject())
            {
                stream.Name(ref TemplateName);
            }
        }

        /// <summary>
        /// Attempts to serialize the properties of this object using the object's class definition. If the definition
        /// can't be found, falls back to <see cref="SerializeTaggedProperties(List{USerializedProperty}, IUnrealDataStream)"/>.
        /// </summary>
        /// <param name="props">A list of properties, which will be either read from or written to.</param>
        /// <param name="stream">The stream to read from or write to.</param>
        protected virtual void SerializeScriptProperties(List<USerializedProperty> props, IUnrealDataStream stream)
        {
            UClass classObj = TableEntry.ClassObj;

            if (classObj != null)
            {
                TableEntry.ClassObj.SerializeTaggedProperties(props, stream);
            }
            else
            {
                // If we don't have a class object, this is likely an intrinsic type, and we'll just have to do the best we can
                SerializeTaggedProperties(props, stream);
            }
        }

        /// <summary>
        /// Serializes the tagged properties of this object, without relying on the object's class definition. Should only be
        /// used if the class definition is unavailable for some reason, as this is much less robust.
        /// </summary>
        /// <param name="props"></param>
        /// <param name="stream"></param>
        public virtual void SerializeTaggedProperties(List<USerializedProperty> props, IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                FPropertyTag tag = new FPropertyTag();
                stream.Object(ref tag);

                while (!tag.Name.IsNone())
                {
                    USerializedProperty prop = ChooseSerializedPropertyBasedOnTag(tag, stream.Archive!);

                    if (prop == null)
                    {
                        Log.Error($"{FullObjectPath} property {tag.Name}: couldn't identify property class {tag.Type}. Skipping {tag.Size} bytes");
                        stream.SkipBytes(tag.Size);
                        continue;
                    }

                    prop.Serialize(stream);
                    props.Add(prop);

                    stream.Object(ref tag);
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    if (props[i].Tag != null)
                    {
                        var tag = props[i].Tag!.Value;
                        stream.Object(ref tag);
                    }

                    props[i].Serialize(stream);
                }

                var nameNone = stream.Archive.GetOrCreateName("None");
                stream.Name(ref nameNone);
            }
        }

        /// <summary>
        /// Chooses a type of <see cref="USerializedProperty"/> based on the given property tag; only used when
        /// the class definition of this object is not available.
        /// </summary>
        protected virtual USerializedProperty ChooseSerializedPropertyBasedOnTag(FPropertyTag tag, FArchive inArchive)
        {
            if (tag.IsStructProperty && UStructProperty.TryCreateSerializedStructProperty(inArchive, tag, out USerializedProperty prop))
            {
                return prop;
            }

            return (string) tag.Type switch
            {
                "ArrayProperty" => new USerializedArrayProperty(inArchive, null, tag),
                "BoolProperty" => new USerializedBoolProperty(inArchive, null, tag),
                "ByteProperty" => new USerializedByteProperty(inArchive, null, tag),
                "ClassProperty" => new USerializedObjectProperty(inArchive, null, tag),
                "ComponentProperty" => new USerializedObjectProperty(inArchive, null, tag),
                "DelegateProperty" => new USerializedDelegateProperty(inArchive, null, tag),
                "FloatProperty" => new USerializedFloatProperty(inArchive, null, tag),
                "IntProperty" => new USerializedIntProperty(inArchive, null, tag),
                "NameProperty" => new USerializedNameProperty(inArchive, null, tag),
                "ObjectProperty" => new USerializedObjectProperty(inArchive, null, tag),
                "StrProperty" => new USerializedStringProperty(inArchive, null, tag),
                "StructProperty" => new USerializedStructProperty(inArchive, null, tag),
                _ => null,
            };
        }

        private bool IsOwnedByClassDefaultObject()
        {
            UObject outer = this;

            while (outer != null)
            {
                if (outer.ExportTableEntry.ObjectFlags.HasFlag(ObjectFlag.ClassDefaultObject))
                {
                    return true;
                }

                if (outer.TableEntry.OuterIndex != 0)
                {
                    outer = Archive.GetObjectByIndex(outer.TableEntry.OuterIndex);
                }
                else
                {
                    outer = null;
                }
            }

            return false;
        }

        public static UObject NewObjectBasedOnClassName(string className, FArchive archive, FObjectTableEntry tableEntry)
        {
            if (ClassNameToUObjectFactory.TryGetValue(className, out Func<FArchive, FObjectTableEntry, UObject>? factory))
            {
                return factory(archive, tableEntry);
            }

            // Anything which doesn't fall into the special cases is an object of an arbitrary Unreal type, which are all handled the same
            return new UObject(archive, tableEntry);
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{GetType()}: {TableEntry.FullObjectPath}";
            }
        }
    }
}
