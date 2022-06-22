using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UniKit.Editor
{
    public class InstalledPackages : AssetPostprocessor
    {
        public enum ModificationType
        {
            Imported,
            Deleted,
            Moved,
            MovedFromAsset
        }
        //private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        //{
        //    return;
        //    var customPackageSources = (PackageSource.Git | PackageSource.Local | PackageSource.Unknown | PackageSource.Embedded);
        //    //WIP
        //    HandlePaths(importedAssets, ModificationType.Imported);
        //    HandlePaths(deletedAssets, ModificationType.Deleted);
        //    HandlePaths(movedAssets, ModificationType.Moved);
        //    HandlePaths(movedFromAssetPaths, ModificationType.MovedFromAsset);

        //    bool TryGetChangedPackage(string path, out PackageInfo package)
        //    {
        //        //Check if the package is a custom package
        //        if (!path.StartsWith("Packages/"))
        //        {
        //            package = default;
        //            return false;
        //        }

        //        package = PackageInfo.FindForAssetPath(path);

        //        if (package == null || !customPackageSources.HasFlag(package.source))
        //            return false;

        //        return true;
        //    }
        //    void HandlePaths(string[] paths, ModificationType modificationType)
        //    {
        //        foreach (var path in paths)
        //            if (!string.IsNullOrWhiteSpace(path) && TryGetChangedPackage(path, out var package))
        //                SendCallbacks(package, path, modificationType);
        //    }
        //}
        private static void SendCallbacks(PackageInfo package, string path, ModificationType modificationType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (PackageInfo.FindForAssembly(assembly) != package)
                    continue;

                assembly.GetTypes();
            }
        }
    }
}