﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal
{
    public class FURL : IUnrealSerializable
    {
        #region Serialized data

        public string Protocol;

        public string Host;
        
        public string Map;
        
        public string Portal;
        
        public string[] Op;
        
        public int Port;
        
        public int Valid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.String(ref Protocol);
            stream.String(ref Host);
            stream.String(ref Map);
            stream.String(ref Portal);
            stream.StringArray(ref Op);
            stream.Int32(ref Port);
            stream.Int32(ref Valid);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FURL) sourceObj;

            Protocol = other.Protocol;
            Host = other.Host;
            Map = other.Map;
            Portal = other.Portal;
            Op = other.Op;
            Port = other.Port;
            Valid = other.Valid;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
