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
            stream.Name(ref BoneName);

            if (stream.IsRead)
            {
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
            }

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

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            USerializedAimComponentProperty other = (USerializedAimComponentProperty) sourceProp;

            BoneName = Archive.MapNameFromSourceArchive(other.BoneName);
            LU = other.LU;
            LC = other.LC;
            LD = other.LD;
            CU = other.CU;
            CC = other.CC;
            CD = other.CD;
            RU = other.RU;
            RC = other.RC;
            RD = other.RD;
        }
    }
}
