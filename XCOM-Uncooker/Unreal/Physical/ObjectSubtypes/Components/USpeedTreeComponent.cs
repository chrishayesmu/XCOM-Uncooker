﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Lighting;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public class USpeedTreeComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FLightMap BranchLightMap = new FLightMap();
        public FLightMap FrondLightMap = new FLightMap();
        public FLightMap LeafCardLightMap = new FLightMap(); 
        public FLightMap BillboardLightMap = new FLightMap();
		public FLightMap LeafMeshLightMap = new FLightMap();

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref BranchLightMap);
            stream.Object(ref FrondLightMap);
            stream.Object(ref LeafCardLightMap);
            stream.Object(ref BillboardLightMap);
            stream.Object(ref LeafMeshLightMap);
        }
    }
}
