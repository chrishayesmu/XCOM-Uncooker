using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedAimOffsetProfileProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

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
                stream.Name(out ProfileName);
                stream.Bytes(out BinaryData, 16);

                stream.Int32(out int numAimComponents);

                AimComponents = new USerializedAimComponentProperty[numAimComponents];

                for (int i = 0; i < numAimComponents; i++)
                {
                    // Hacky, but USerializedAimComponentProperty doesn't care about its prop or tag
                    AimComponents[i] = new USerializedAimComponentProperty(Archive, BackingProperty, null);
                    AimComponents[i].Serialize(stream);
                }

                stream.Name(out AnimName_LU);
                stream.Name(out AnimName_LC);
                stream.Name(out AnimName_LD);
                stream.Name(out AnimName_CU);
                stream.Name(out AnimName_CC);
                stream.Name(out AnimName_CD);
                stream.Name(out AnimName_RU);
                stream.Name(out AnimName_RC);
                stream.Name(out AnimName_RD);
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

                stream.PropertyTag(out tag);
                stream.Name(out ProfileName);

                tag.Name = Archive.GetOrCreateName("HorizontalRange");
                tag.Type = NAME_StructProperty;
                tag.StructName = Archive.GetOrCreateName("Vector2D");

                stream.PropertyTag(out tag);
                stream.Bytes(out BinaryData, tag.Size);

                tag.Name = Archive.GetOrCreateName("VerticalRange");

                stream.PropertyTag(out tag);
                stream.Bytes(out BinaryData, tag.Size, 8);

                // AimComponents sizing:
                //     4 bytes: array size
                //     N * sizeof(USerializedAimComponentProperty): data
                tag.Name = Archive.GetOrCreateName("AimComponents");
                tag.Type = Archive.GetOrCreateName("ArrayProperty");
                tag.StructName = null;
                tag.Size = 4 + AimComponents.Length * USerializedAimComponentProperty.TaggedPropertiesSize;

                int numAimComponents = AimComponents.Length;
                stream.PropertyTag(out tag);
                stream.Int32(out numAimComponents);

                for (int i = 0; i < numAimComponents; i++)
                {
                    // For a dynamic array, there's no top-level tag; we launch right into the serialized item
                    AimComponents[i].Serialize(stream);
                }

                stream.Name(out AnimName_LU);
                stream.Name(out AnimName_LC);
                stream.Name(out AnimName_LD);
                stream.Name(out AnimName_CU);
                stream.Name(out AnimName_CC);
                stream.Name(out AnimName_CD);
                stream.Name(out AnimName_RU);
                stream.Name(out AnimName_RC);
                stream.Name(out AnimName_RD);
            }
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            USerializedAimOffsetProfileProperty other = (USerializedAimOffsetProfileProperty) sourceProp;

            ProfileName = Archive.MapNameFromSourceArchive(other.ProfileName);

            FPropertyTag tag = new FPropertyTag()
            {
                Name = Archive.GetOrCreateName("HorizontalRange"),
                Type = Archive.GetOrCreateName("StructProperty"),
                ArrayIndex = 0,
                Size = 8,
                StructName = Archive.GetOrCreateName("Vector2D")
            };

            AimComponents = new USerializedAimComponentProperty[other.AimComponents.Length];

            for (int i = 0; i < AimComponents.Length; i++)
            {
                // TODO this is going to be the wrong backing property - does it even matter though?
                AimComponents[i] = new USerializedAimComponentProperty(Archive, BackingProperty, tag);
                AimComponents[i].CloneFromOtherArchive(other.AimComponents[i]);
            }

            AnimName_LU = Archive.MapNameFromSourceArchive(other.AnimName_LU);
            AnimName_LC = Archive.MapNameFromSourceArchive(other.AnimName_LC);
            AnimName_LD = Archive.MapNameFromSourceArchive(other.AnimName_LD);
            AnimName_CU = Archive.MapNameFromSourceArchive(other.AnimName_CU);
            AnimName_CC = Archive.MapNameFromSourceArchive(other.AnimName_CC);
            AnimName_CD = Archive.MapNameFromSourceArchive(other.AnimName_CD);
            AnimName_RU = Archive.MapNameFromSourceArchive(other.AnimName_RU);
            AnimName_RC = Archive.MapNameFromSourceArchive(other.AnimName_RC);
            AnimName_RD = Archive.MapNameFromSourceArchive(other.AnimName_RD);
        }
    }
}
