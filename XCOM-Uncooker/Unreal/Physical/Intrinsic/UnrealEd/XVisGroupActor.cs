using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.UnrealEd
{
    /// <summary>
    /// <c>VisGroupActor</c> appears to be some kind of XCOM-added intrinsic class in UnrealEd. While the same class
    /// exists as an ordinary class in XCOM 2, it isn't clear that the properties are the same between the two games.
    /// In particular, XCOM: EW's <c>VisGroupActor</c> has 22 bytes of data serialized before its <see cref="UObject"/> data.
    /// </summary>
    public class XVisGroupActor(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public int Index1; // unknown, so far it always points at the UnrealEd.VisGroupActor class import

        public int Index2; // unknown, so far always matches Index1

        public int IntValue1; // unknown, so far always -1

        public byte[] ByteData; // unknown; 6 bytes long. First 4 bytes may be an int or enum value, not clear. 
                                // So far all bytes are 0, except the first byte is nonzero and has been 0x54 and 0x69.

        public int IntValue2; // unknown, so far always -1

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Index1);
            stream.Int32(out Index2);
            stream.Int32(out IntValue1);
            stream.Bytes(out ByteData, 6);
            stream.Int32(out IntValue2);

            // TODO: every instance of VisGroupActor has the HasStack flag set, but some (all?) of them don't actually have stack frames, and it's breaking UObject.Serialize
            return;

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
