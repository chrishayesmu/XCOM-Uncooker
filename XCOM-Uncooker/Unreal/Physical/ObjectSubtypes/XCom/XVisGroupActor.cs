using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.XCom
{
    /// <summary>
    /// <c>VisGroupActor</c> appears to be some kind of XCOM-added intrinsic class in UnrealEd. While the same class
    /// exists as an ordinary class in XCOM 2, it isn't clear that the properties are the same between the two games.
    /// In particular, XCOM: EW's <c>VisGroupActor</c> has 22 bytes of data serialized before its <see cref="UObject"/> data.
    /// </summary>
    public class XVisGroupActor(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UClass))]
        public int ClassIndex1; // always points to class'UnrealEd.VisGroupActor'

        [Index(typeof(UClass))]
        public int ClassIndex2; // always points to class'UnrealEd.VisGroupActor'

        public int UnknownValue1; // so far, always -1
        public int UnknownValue2; // varies, low values (less than 200 so far), unclear meaning

        public byte UnknownByte1; // so far, always 0
        public byte UnknownByte2; // so far, always 0

        public int UnknownValue3; // so far, always -1

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out ClassIndex1);
            stream.Int32(out ClassIndex2);
            stream.Int32(out UnknownValue1);
            stream.Int32(out UnknownValue2);
            stream.UInt8(out UnknownByte1);
            stream.UInt8(out UnknownByte2);
            stream.Int32(out UnknownValue3);

#if DEBUG
            if (ClassIndex1 != ClassIndex2 || UnknownValue1 != -1 || UnknownByte1 != 0 || UnknownByte2 != 0 || UnknownValue3 != -1)
            {
                Debugger.Break();
            }
#endif

            base.Serialize(stream);
        }

        protected override USerializedProperty ChooseSerializedPropertyBasedOnTag(FPropertyTag tag)
        {
            // For this property:
            //   var() array<VisGroupActor> DependentOn;
            if (tag.Name == "DependentOn")
            {
                return new USerializedIndexArrayProperty(Archive, null, tag);
            }

            return base.ChooseSerializedPropertyBasedOnTag(tag);
        }
    }
}
