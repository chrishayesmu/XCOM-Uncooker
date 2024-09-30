using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes;

namespace UnrealArchiveLibrary.Unreal.Shaders
{
    public class GlobalShaderFile
    {
        public const uint SIGNATURE = 0x47534D42;

        public FShaderCache ShaderCache = new FShaderCache();

        public IDictionary<FShaderType, FShaderPtr> Shaders = new Dictionary<FShaderType, FShaderPtr>();

        public GlobalShaderFile(IUnrealDataStream stream)
        {
            if (!stream.IsRead)
            {
                throw new ArgumentException("Only read is supported for GlobalShaderFile");
            }

            uint signature = 0;
            stream.UInt32(ref signature);

            if (signature != SIGNATURE)
            {
                throw new Exception($"Input stream has invalid signature: {signature:X}");
            }

            int version = 0, licenseeVersion = 0;
            stream.Int32(ref version);
            stream.Int32(ref licenseeVersion);

            if (version != 845 || licenseeVersion != 64)
            {
                throw new Exception($"Expected file version 845:64, but got {version}:{licenseeVersion}");
            }

            stream.Object(ref ShaderCache);
            stream.Map(ref Shaders);
        }
    }
}
