using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker.Unreal.Physical
{
    [DebuggerDisplay("{ToString()}")]
    public class FName
    {
        public FName() { }

        /// <summary>
        /// Index into the name table where this FName's value can be found.
        /// </summary>
        public int Index = -1;

        public int Suffix;

        public FArchive Archive;

        // NOTE: two FNames are equivalent only if they have the same indices and come from the same archive!
        // Use string comparisons explicitly if they should be compared as strings

        public static bool operator ==(FName name, FName other)
        {
            if (name is null && other is null) 
            { 
                return true; 
            }

            if (name is null || other is null)
            {
                return false;
            }

            // Try to compare as just indices if they're in the same archive, for efficiency
            if (name.Archive == other.Archive)
            {
                return name.Index == other.Index && name.Suffix == other.Suffix;
            }

            // Fall back to string comparison if they're in different archives
            return name.ToString().ToLower() == other.ToString().ToLower();
        }

        public static bool operator !=(FName name, FName other) => !(name == other);

        public static bool operator ==(FName name, string str) => name.ToString() == str;
        public static bool operator !=(FName name, string str) => !(name == str);

        public static implicit operator string(FName name) => name.ToString();

        public string WithoutSuffix()
        {
            return Archive.NameTable[Index];
        }

        public bool IsNone()
        {
            return Index < 0 || this == "None";
        }

        public override string ToString()
        {
            return Archive.NameToString(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is FName name && ToString() == name.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
