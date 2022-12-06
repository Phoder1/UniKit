using System;
using System.Collections.Generic;
using System.Linq;
using UniKit.Reflection;
using UnityEditor;
using UnityEditor.Compilation;

namespace UniKit.Editor
{
    using AssemblyName = System.Reflection.AssemblyName;
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;
    public static class EditorReflectionExt
    {

        public static Lazy<IReadOnlyList<UnityEditor.Compilation.Assembly>> unityPlayerAssemblies = new(GetPlayerUnityAssemblies);
        public static IReadOnlyList<UnityEditor.Compilation.Assembly> UnityPlayerAssemblies => unityPlayerAssemblies.Value;

        public static Lazy<IReadOnlyList<UnityEditor.Compilation.Assembly>> unityEditorAssemblies = new(GetEditorUnityAssemblies);
        public static IReadOnlyList<UnityEditor.Compilation.Assembly> UnityEditorAssemblies => unityEditorAssemblies.Value;

        public static Lazy<IReadOnlyList<System.Reflection.Assembly>> systemPlayerAssemblies = new(UnityPlayerAssemblies.GetSystemAssemblies().ToList);
        public static IReadOnlyList<System.Reflection.Assembly> SystemPlayerAssemblies => systemPlayerAssemblies.Value;
        private static Lazy<IReadOnlyDictionary<Type, IEnumerable<Type>>> CustomEditors = new Lazy<IReadOnlyDictionary<Type, IEnumerable<Type>>>(LoadCustomEditors);

        private static Dictionary<Type, IEnumerable<Type>> LoadCustomEditors() 
            => typeof(UnityEditor.Editor).GetAllThatImplement(true)
                                         .WithAttribute<CustomEditor>()
                                         .Select(p => new KeyValuePair<Type, Type>(p.Key, p.Value.GetInspectedType()))
                                         .Swap();
        public static IEnumerable<Type> GetCustomEditorTypes(this Type type) => CustomEditors.Value.GetValueOrDefault(type);
        public static Type GetCustomEditorType(this Type type) => type.GetCustomEditorTypes()?.FirstOrDefault();
        public static Editor CreateEditor(this Object obj) => Editor.CreateEditor(obj, obj.GetType().GetCustomEditorTypes()?.FirstOrDefault());
        public static void CreateCachedEditor(this Object obj, ref Editor prevEditor) => Editor.CreateCachedEditor(obj, obj.GetType().GetCustomEditorType(), ref prevEditor);
        private static IReadOnlyList<UnityEditor.Compilation.Assembly> GetPlayerUnityAssemblies() => GetAssemblies(false);
        private static IReadOnlyList<UnityEditor.Compilation.Assembly> GetEditorUnityAssemblies() => GetAssemblies(true);
        private static IReadOnlyList<UnityEditor.Compilation.Assembly> GetAssemblies(bool includeEditor = false)
        {
            return CompilationPipeline.GetAssemblies(includeEditor ? AssembliesType.Editor : AssembliesType.Player).Where(IsValid);

            bool IsValid(UnityEditor.Compilation.Assembly x) => includeEditor || !x.name.StartsWith("Assembly-CSharp-firstpass");
        }

        public static IEnumerable<Type> GetAllThatImplement(this Type type, bool includeEditor = false, bool includeSelf = false)
        {
            return type.GetAllSystemAssembliesThatMightImplement(includeEditor)
                       .SelectMany((x) => x.GetTypes())
                       .Where(IsImplementing);

            bool IsImplementing(Type typeToCheck) => typeToCheck != null && (includeSelf || typeToCheck != type) && type.IsAssignableFromGeneric(typeToCheck);
        }
        public static IEnumerable<System.Reflection.Assembly> GetAllSystemAssembliesThatMightImplement(this Type type, bool includeEditor = false)
            => (includeEditor ? AppDomain.CurrentDomain.GetAssemblies() : SystemPlayerAssemblies).WhereMightImplement(type);
        public static IEnumerable<System.Reflection.Assembly> WhereMightImplement(this IEnumerable<System.Reflection.Assembly> assemblies, Type type) => assemblies.Where(type.Assembly.IsRefrencedBy).Append(type.Assembly);
        public static bool IsRefrencedBy(this System.Reflection.Assembly assembly, IEnumerable<System.Reflection.Assembly> assemblies) => assemblies.Any(a => assembly.IsRefrencedBy(a));
        public static bool IsRefrencedBy(this System.Reflection.Assembly referencedAssembly, System.Reflection.Assembly assemblies)
        {
            var name = referencedAssembly.GetName();
            return assemblies.GetReferencedAssemblies().Any(r => AssemblyName.ReferenceMatchesDefinition(r, name));
        }
        public static string DisplayName(this System.Reflection.Assembly assembly) => assembly.FullName.Split(',')[0];
        public static List<UnityEditor.Compilation.Assembly> GetAllUnityAssembliesThatMightImplement(this Type type, bool includeEditor = false)
        {
            var assembly = type.Assembly;
            return (includeEditor ? UnityEditorAssemblies : UnityPlayerAssemblies).Where(IsValid);

            bool IsValid(UnityEditor.Compilation.Assembly x)
                => assembly.FullName.StartsWith(x.name) || x.HasReferenceTo(assembly);
        }
        public static IEnumerable<System.Reflection.Assembly> GetSystemAssemblies(this IEnumerable<UnityEditor.Compilation.Assembly> assemblies)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(Contains);

            bool Contains(System.Reflection.Assembly assembly) 
                => assemblies.Any((x) => assembly.FullName.StartsWith(x.name));
        }
    }
}
