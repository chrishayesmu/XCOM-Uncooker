using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core
{
    /// <summary>
    /// UPackage is part of the fundamental grouping of objects in UE3. However, this UPackage class is barebones and identical to UObject - all of the 
    /// things typically thought of as being part of a package are found in <see cref="FArchive" />. This is because, within an archive, UPackage basically
    /// just exists to be referenced by other objects.
    /// 
    /// <para>
    /// There are two main ways that UPackages get created in the normal course of using UE3:
    ///   <list type="bullet">
    ///     <item>When creating a new package file in the Unreal Editor. Ex: "Game.upk" creates a package called "Game"</item>
    ///     <item>When creating a new grouping in the Unreal Editor. Ex: "Game.Group.Obj" creates a package "Group" inside of the package "Game"</item>
    ///   </list>  
    /// </para>
    /// 
    /// The UPackage class introduces no extra data, because everything needed can be found in its <see cref="FObjectExport"/> or <see cref="FObjectImport"/>
    /// entry. (All that's really needed is the package's name.) Like UObject, UPackage supports an arbitrary number of properties in its serialized data
    /// block, though I'm not sure if there are any examples of this being used in XCOM.
    /// </summary>
    public class UPackage(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public string NormalizedName
        {
            get
            {
                string name = ObjectName;

                if (name.EndsWith("_SF"))
                {
                    name = name.Substring(0, name.Length - 3);
                }

                return name;
            }
        }
    }
}
