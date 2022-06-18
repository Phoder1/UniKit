using System.Reflection;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Phoder1.Editor
{
    public static class PackageExt
    {
#pragma warning disable
        public static PackageInfo FindContainingPackage<T>(this T obj)
            => FindContainingPackage<T>();
#pragma warning restore
        public static PackageInfo FindContainingPackage<T>()
            => typeof(T).Assembly.FindContainingPackage();
        public static PackageInfo FindContainingPackage(this Assembly assembly)
            => PackageInfo.FindForAssembly(assembly);
    }
}
