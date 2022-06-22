using System;
using System.Reflection;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UniKit.Editor
{
    public static class PackageExt
    {
#pragma warning disable
        public static PackageInfo FindContainingPackage(this Type type)
            => type.Assembly.FindContainingPackage();
        public static PackageInfo FindContainingPackage(this Assembly assembly)
            => PackageInfo.FindForAssembly(assembly);
        public static PackageInfo FindContainingPackage<T>(this T obj)
            => FindContainingPackage<T>();
        public static PackageInfo FindContainingPackage<T>()
            => typeof(T).FindContainingPackage();
#pragma warning restore
    }
}
