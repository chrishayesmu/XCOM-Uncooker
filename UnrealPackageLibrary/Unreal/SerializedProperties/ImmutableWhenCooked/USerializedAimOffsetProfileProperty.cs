using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedAimOffsetProfileProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        public override bool HasDefaultValueForType => false; // doesn't matter; just let it get serialized every time

        #region Serialized data

        public FName ProfileName;

        // Vector2D HorizontalRange
        // Vector2D VerticalRange
        public byte[] BinaryData;

        public USerializedAimComponentProperty[] AimComponents;

        public FName AnimName_LU;
        public FName AnimName_LC;
        public FName AnimName_LD;
        public FName AnimName_CU;
        public FName AnimName_CC;
        public FName AnimName_CD;
        public FName AnimName_RU;
        public FName AnimName_RC;
        public FName AnimName_RD;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.Name(ref ProfileName);
                stream.Bytes(ref BinaryData, 16);

                int numAimComponents = 0;
                stream.Int32(ref numAimComponents);

                AimComponents = new USerializedAimComponentProperty[numAimComponents];

                for (int i = 0; i < numAimComponents; i++)
                {
                    // Hacky, but USerializedAimComponentProperty doesn't care about its prop or tag
                    AimComponents[i] = new USerializedAimComponentProperty(Archive, BackingProperty, null);
                    AimComponents[i].Serialize(stream);
                }

                stream.Name(ref AnimName_LU);
                stream.Name(ref AnimName_LC);
                stream.Name(ref AnimName_LD);
                stream.Name(ref AnimName_CU);
                stream.Name(ref AnimName_CC);
                stream.Name(ref AnimName_CD);
                stream.Name(ref AnimName_RU);
                stream.Name(ref AnimName_RC);
                stream.Name(ref AnimName_RD);
            }
            else
            {
                FName NAME_StructProperty = Archive.GetOrCreateName("StructProperty");

                FPropertyTag tag = new FPropertyTag()
                {
                    Name = Archive.GetOrCreateName("ProfileName"),
                    Type = Archive.GetOrCreateName("NameProperty"),
                    ArrayIndex = 0,
                    Size = 8
                };

                stream.Object(ref tag);
                stream.Name(ref ProfileName);

                tag.Name = Archive.GetOrCreateName("HorizontalRange");
                tag.Type = NAME_StructProperty;
                tag.StructName = Archive.GetOrCreateName("Vector2D");

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, 8);

                tag.Name = Archive.GetOrCreateName("VerticalRange");

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, 8, 8);

                // AimComponents sizing:
                //     4 bytes: array size
                //     N * sizeof(USerializedAimComponentProperty): data
                tag.Name = Archive.GetOrCreateName("AimComponents");
                tag.Type = Archive.GetOrCreateName("ArrayProperty");
                tag.StructName = null;
                tag.Size = 4 + AimComponents.Length * USerializedAimComponentProperty.TaggedPropertiesSize;

                int numAimComponents = AimComponents.Length;
                stream.Object(ref tag);
                stream.Int32(ref numAimComponents);

                for (int i = 0; i < numAimComponents; i++)
                {
                    // For a dynamic array, there's no top-level tag; we launch right into the serialized item
                    AimComponents[i].Serialize(stream);
                }

                tag.Type = Archive.GetOrCreateName("NameProperty");
                tag.Size = 8;

                tag.Name = Archive.GetOrCreateName("AnimName_LU");
                stream.Object(ref tag);
                stream.Name(ref AnimName_LU);

                tag.Name = Archive.GetOrCreateName("AnimName_LC");
                stream.Object(ref tag);
                stream.Name(ref AnimName_LC);

                tag.Name = Archive.GetOrCreateName("AnimName_LD");
                stream.Object(ref tag);
                stream.Name(ref AnimName_LD);

                tag.Name = Archive.GetOrCreateName("AnimName_CU");
                stream.Object(ref tag);
                stream.Name(ref AnimName_CU);

                tag.Name = Archive.GetOrCreateName("AnimName_CC");
                stream.Object(ref tag);
                stream.Name(ref AnimName_CC);

                tag.Name = Archive.GetOrCreateName("AnimName_CD");
                stream.Object(ref tag);
                stream.Name(ref AnimName_CD);

                tag.Name = Archive.GetOrCreateName("AnimName_RU");
                stream.Object(ref tag);
                stream.Name(ref AnimName_RU);

                tag.Name = Archive.GetOrCreateName("AnimName_RC");
                stream.Object(ref tag);
                stream.Name(ref AnimName_RC);

                tag.Name = Archive.GetOrCreateName("AnimName_RD");
                stream.Object(ref tag);
                stream.Name(ref AnimName_RD);

                FName NAME_None = Archive.GetOrCreateName("None");
                stream.Name(ref NAME_None);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedAimOffsetProfileProperty(destArchive, null, tag);

            other.ProfileName = destArchive.MapNameFromSourceArchive(ProfileName);
            other.BinaryData = BinaryData;

            other.AimComponents = new USerializedAimComponentProperty[AimComponents.Length];

            for (int i = 0; i < AimComponents.Length; i++)
            {
                other.AimComponents[i] = (USerializedAimComponentProperty) AimComponents[i].CloneToOtherArchive(destArchive);
            }

            other.AnimName_LU = destArchive.MapNameFromSourceArchive(AnimName_LU);
            other.AnimName_LC = destArchive.MapNameFromSourceArchive(AnimName_LC);
            other.AnimName_LD = destArchive.MapNameFromSourceArchive(AnimName_LD);
            other.AnimName_CU = destArchive.MapNameFromSourceArchive(AnimName_CU);
            other.AnimName_CC = destArchive.MapNameFromSourceArchive(AnimName_CC);
            other.AnimName_CD = destArchive.MapNameFromSourceArchive(AnimName_CD);
            other.AnimName_RU = destArchive.MapNameFromSourceArchive(AnimName_RU);
            other.AnimName_RC = destArchive.MapNameFromSourceArchive(AnimName_RC);
            other.AnimName_RD = destArchive.MapNameFromSourceArchive(AnimName_RD);

            // Ensure all of these names exist in the destination archive
            destArchive.GetOrCreateName("AimComponents");
            destArchive.GetOrCreateName("ArrayProperty");
            destArchive.GetOrCreateName("HorizontalRange");
            destArchive.GetOrCreateName("NameProperty");
            destArchive.GetOrCreateName("None");
            destArchive.GetOrCreateName("ProfileName");
            destArchive.GetOrCreateName("StructProperty");
            destArchive.GetOrCreateName("Vector2D");
            destArchive.GetOrCreateName("VerticalRange");
            destArchive.GetOrCreateName("AnimName_LU");
            destArchive.GetOrCreateName("AnimName_LC");
            destArchive.GetOrCreateName("AnimName_LD");
            destArchive.GetOrCreateName("AnimName_CU");
            destArchive.GetOrCreateName("AnimName_CC");
            destArchive.GetOrCreateName("AnimName_CD");
            destArchive.GetOrCreateName("AnimName_RU");
            destArchive.GetOrCreateName("AnimName_RC");
            destArchive.GetOrCreateName("AnimName_RD");

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
