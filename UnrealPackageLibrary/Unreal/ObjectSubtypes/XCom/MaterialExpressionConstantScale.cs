using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.XCom
{
    /// <summary>
    /// Expression from XCOM 2 (and possibly XCOM EW) which simply multiplies the input expression by a constant factor.
    /// </summary>
    public class MaterialExpressionConstantScale(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        // TODO: when uncooking this, transform it into a Multiply node + Constant node
    }
}
