using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Flash
{
    public class USwfMovie(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public byte[] Data;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            // XCOM appears to have changed serialization of SwfMovie files: rather than using the RawData property,
            // they stick the Flash data at the end of the object. When reading, we pull from that block; when writing,
            // we write the data to the RawData property.
            if (stream.IsRead)
            {
                stream.ByteArray(ref Data);
            }
            else
            {
                // The stream will have just written the name "None" to indicate the end of tagged properties. Go back a little
                // and overwrite that, then we'll reappend it ourselves.
                stream.Seek(-8, SeekOrigin.Current);

                var rawDataTag = new FPropertyTag()
                {
                    Name = Archive.GetOrCreateName("RawData"),
                    Type = Archive.GetOrCreateName("ArrayProperty"),
                    Size = 4 + Data.Length, // 4 extra bytes for the array's length
                    ArrayIndex = 0
                };

                stream.Object(ref rawDataTag);
                stream.ByteArray(ref Data);

                FName NAME_None = Archive.GetOrCreateName("None");
                stream.Name(ref NAME_None);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (USwfMovie) sourceObj;

            Data = other.Data;

            Archive.GetOrCreateName("ArrayProperty");
            Archive.GetOrCreateName("RawData");
        }
    }
}
