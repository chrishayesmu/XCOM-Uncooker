using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;

namespace UnrealArchiveLibrary.Unreal
{
    public interface IUnrealSerializable
    {
        /// <summary>
        /// This object's owner, somewhere up the hierarchy. Note that most objects don't care who their owner
        /// is and won't implement this property, leaving it to be null all the time. Only a few special situations
        /// bother to implement this.
        /// </summary>
        public UObject Owner 
        {
            get => null;
            set { }
        }

        public static T[] Clone<T>(T[] data, FArchive sourceArchive, FArchive destArchive) where T : IUnrealSerializable, new()
        {
            T[] output = new T[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                output[i] = new T();
                output[i].CloneFromOtherArchive(data[i], sourceArchive, destArchive);
            }

            return output;
        }

        public static IList<T> Clone<T>(IList<T> data, FArchive sourceArchive, FArchive destArchive) where T : IUnrealSerializable, new()
        {
            var output = new List<T>(data.Count);

            for (int i = 0; i < data.Count; i++)
            {
                var obj = new T();
                obj.CloneFromOtherArchive(data[i], sourceArchive, destArchive);
                output.Add(obj);
            }

            return output;
        }

        public void Serialize(IUnrealDataStream stream);

        /// <summary>
        /// Causes this object to clone its data from another object, which will be of the same type.
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="sourceArchive"></param>
        /// <param name="destArchive"></param>
        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive);

        /// <summary>
        /// Populates the given list with any object indices (both exports and imports) which this object is
        /// dependent upon.
        /// </summary>
        /// <param name="dependencyIndices">A list of dependencies to add to.</param>
        public void PopulateDependencies(List<int> dependencyIndices);
    }
}
