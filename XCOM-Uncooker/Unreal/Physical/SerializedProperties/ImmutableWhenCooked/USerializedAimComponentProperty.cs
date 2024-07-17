using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedAimComponentProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        public const int TaggedPropertiesSize = 8 + 24 // BoneName + its property tag
                                              + 9 * 32 // Property tags for each of the 9 USerializedAimTransformProperty instances
                                              + 9 * USerializedAimTransformProperty.TaggedPropertiesSize
                                              + 8; // NAME_None to end the property block

        #region Serialized data

        public FName BoneName;

        public USerializedAimTransformProperty LU;
        public USerializedAimTransformProperty LC;
        public USerializedAimTransformProperty LD;
        public USerializedAimTransformProperty CU;
        public USerializedAimTransformProperty CC;
        public USerializedAimTransformProperty CD;
        public USerializedAimTransformProperty RU;
        public USerializedAimTransformProperty RC;
        public USerializedAimTransformProperty RD;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.Name(ref BoneName);

                // Bit hacky, but USerializedAimTransformProperty doesn't actually care what its prop says
                LU = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                LC = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                LD = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                CU = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                CC = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                CD = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                RU = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                RC = new USerializedAimTransformProperty(Archive, BackingProperty, null);
                RD = new USerializedAimTransformProperty(Archive, BackingProperty, null);

                LU.Serialize(stream);
                LC.Serialize(stream);
                LD.Serialize(stream);
                CU.Serialize(stream);
                CC.Serialize(stream);
                CD.Serialize(stream);
                RU.Serialize(stream);
                RC.Serialize(stream);
                RD.Serialize(stream);
            }
            else
            {
                // Prefix BoneName with its property tag
                FPropertyTag tag = new FPropertyTag()
                {
                    Name = Archive.GetOrCreateName("BoneName"),
                    Type = Archive.GetOrCreateName("NameProperty"),
                    ArrayIndex = 0,
                    Size = 8
                };

                stream.Object(ref tag);
                stream.Name(ref BoneName);

                tag.Type = Archive.GetOrCreateName("StructProperty");
                tag.StructName = Archive.GetOrCreateName("AimTransform");
                tag.Size = USerializedAimTransformProperty.TaggedPropertiesSize;

                tag.Name = Archive.GetOrCreateName("LU");
                stream.Object(ref tag);
                LU.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("LC");
                stream.Object(ref tag);
                LC.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("LD");
                stream.Object(ref tag);
                LD.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("CU");
                stream.Object(ref tag);
                CU.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("CC");
                stream.Object(ref tag);
                CC.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("CD");
                stream.Object(ref tag);
                CD.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("RU");
                stream.Object(ref tag);
                RU.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("RC");
                stream.Object(ref tag);
                RC.Serialize(stream);

                tag.Name = Archive.GetOrCreateName("RD");
                stream.Object(ref tag);
                RD.Serialize(stream);

                FName NAME_None = Archive.GetOrCreateName("None");
                stream.Name(ref NAME_None);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedAimComponentProperty(destArchive, null, tag);

            other.BoneName = destArchive.MapNameFromSourceArchive(BoneName);
            other.LU = (USerializedAimTransformProperty) LU.CloneToOtherArchive(destArchive);
            other.LC = (USerializedAimTransformProperty) LC.CloneToOtherArchive(destArchive);
            other.LD = (USerializedAimTransformProperty) LD.CloneToOtherArchive(destArchive);
            other.CU = (USerializedAimTransformProperty) CU.CloneToOtherArchive(destArchive);
            other.CC = (USerializedAimTransformProperty) CC.CloneToOtherArchive(destArchive);
            other.CD = (USerializedAimTransformProperty) CD.CloneToOtherArchive(destArchive);
            other.RU = (USerializedAimTransformProperty) RU.CloneToOtherArchive(destArchive);
            other.RC = (USerializedAimTransformProperty) RC.CloneToOtherArchive(destArchive);
            other.RD = (USerializedAimTransformProperty) RD.CloneToOtherArchive(destArchive);

            // Ensure all of these names exist in the destination archive
            destArchive.GetOrCreateName("AimTransform");
            destArchive.GetOrCreateName("BoneName");
            destArchive.GetOrCreateName("NameProperty");
            destArchive.GetOrCreateName("StructProperty");
            destArchive.GetOrCreateName("None");
            destArchive.GetOrCreateName("LU");
            destArchive.GetOrCreateName("LC");
            destArchive.GetOrCreateName("LD");
            destArchive.GetOrCreateName("CU");
            destArchive.GetOrCreateName("CC");
            destArchive.GetOrCreateName("CD");
            destArchive.GetOrCreateName("RU");
            destArchive.GetOrCreateName("RC");
            destArchive.GetOrCreateName("RD");

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
