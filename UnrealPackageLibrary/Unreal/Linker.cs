using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Utils;

namespace UnrealArchiveLibrary.Unreal
{
    public class Linker(ILogger logger) : IDisposable
    {
        public List<FArchive> Archives = [];

        public IDictionary<string, MultiValueDictionary<string, UObject>> ObjectsByUncookedArchiveName = new Dictionary<string, MultiValueDictionary<string, UObject>>();
        public IDictionary<string, string> UncookedArchiveNameByObjectPath = new Dictionary<string, string>();
        public IDictionary<Guid, UPackage> InputPackagesByGuid = new Dictionary<Guid, UPackage>();

        private IDictionary<string, FileStream> TextureFileCaches = new Dictionary<string, FileStream>();

        public bool TryGetArchiveWithFileName(string name, out FArchive? archive)
        {
            archive = Archives.SingleOrDefault(ar => ar.FileName == name);

            return archive != null;
        }

        public bool TryGetArchiveWithNormalizedName(string name, out FArchive? archive)
        {
            archive = Archives.SingleOrDefault(ar => ar.NormalizedName == name);

            return archive != null;
        }

        /// <summary>
        /// Finds the object specified by the given path, as long as it exists in at least one of the currently-loaded archives.
        /// </summary>
        /// <param name="fullObjectPath">The full object path, as from <see cref="FObjectTableEntry.FullObjectPath"/>.</param>
        /// <param name="tableEntry">Optionally, a table entry for an existing copy of the object. If provided, it will be used
        /// to help disambiguate scenarios where the same object path is used for multiple objects.</param>
        /// <returns>The corresponding <see cref="UObject"/> if found, else null.</returns>
        /// <remarks>
        /// The return value of this is ambiguous if there are multiple objects with the same path. UE3 itself seems to
        /// have the same problem, and explicitly recommends against naming two objects with the same path. For uncooking, this
        /// will occur naturally because the same logical object has been pulled into multiple archives; we simply assume that
        /// all copies of the object are the same and use the first match we find. The exception is if the tableEntry param is set,
        /// in which case we only match objects if they have the same class as the table entry.
        /// </remarks>
        public UObject GetCookedObjectByPath(string fullObjectPath, FObjectTableEntry tableEntry = null)
        {
            foreach (var archive in Archives)
            {
                var obj = archive.GetExportedObjectByPath(fullObjectPath, tableEntry);

                if (obj != null)
                {
                    if (tableEntry != null)
                    {
                        if (obj.TableEntry.ClassName != tableEntry.ClassName)
                        {
                            continue;
                        }
                    }

                    return obj;
                }
            }

            return null;
        }

        public string GetTopLevelPackageName(FObjectTableEntry tableEntry)
        {
            var outermost = tableEntry.Outermost;

            if (outermost is FImportTableEntry importEntry)
            {
                return importEntry.ObjectName;
            }
            else
            {
                FExportTableEntry exportEntry = outermost as FExportTableEntry;

                // PackageGuid should only be set for top-level package objects
                if (InputPackagesByGuid != null && exportEntry.PackageGuid != Guid.Empty)
                {
                    if (InputPackagesByGuid.TryGetValue(exportEntry.PackageGuid, out UPackage pkg))
                    {
                        return pkg.NormalizedName;
                    }
                }

                // If we got here, either there's no GUID or it's unrecognized; we hope it's the former
                if (exportEntry.IsPackage)
                {
                    return exportEntry.ObjectName;
                }

                // A top-level non-package object belongs to the implied package of its archive name; normally this
                // should only happen for maps, classes, and maybe weird situations in Core/Engine (I think?)
                return exportEntry.Archive.NormalizedName;
            }
        }

        /// <summary>
        /// Finds the name of the archive which will ultimately contain the given object once it is uncooked.
        /// </summary>
        /// <param name="fullObjectPath"></param>
        /// <returns>The archive's name, or an empty string if no match is found.</returns>
        public string GetUncookedArchiveNameForObject(string fullObjectPath)
        {
            if (UncookedArchiveNameByObjectPath.TryGetValue(fullObjectPath, out string archiveName))
            {
                return archiveName;
            }

            foreach (var entry in ObjectsByUncookedArchiveName)
            {
                if (entry.Value.ContainsKey(fullObjectPath))
                {
                    return entry.Key;
                }
            }

            if (fullObjectPath.Contains('.'))
            {
                return fullObjectPath.Split('.')[0];
            }

            return "";
        }

        public byte[] ReadTextureData(string textureFileName, int startPosition, int numBytes)
        {
            if (TextureFileCaches.TryGetValue(textureFileName, out FileStream stream))
            {
                byte[] buffer = new byte[numBytes];

                lock (stream)
                {
                    stream.Seek(startPosition, SeekOrigin.Begin);
                    stream.Read(buffer, 0, numBytes);

                    return buffer;
                }
            }
            
            throw new ArgumentException($"Unrecognized TFC file requested: {textureFileName}");
        }

        public void RegisterTextureFileCache(string textureFileName, string filePath)
        {
            TextureFileCaches[textureFileName] = File.OpenRead(filePath);
        }

        public void Dispose()
        {
            foreach (var archive in Archives)
            {
                archive?.EndSerialization();
            }
        }
    }
}
