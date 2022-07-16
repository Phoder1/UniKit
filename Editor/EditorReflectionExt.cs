using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniKit.Reflection;
using UnityEditor.Compilation;

namespace UniKit.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;
    public static class EditorReflectionExt
    {

        public static Lazy<IReadOnlyList<UnityEditor.Compilation.Assembly>> unityPlayerAssemblies = new(GetPlayerAssemblies);
        public static IReadOnlyList<UnityEditor.Compilation.Assembly> UnityPlayerAssemblies => unityPlayerAssemblies.Value;

        public static Lazy<IReadOnlyList<System.Reflection.Assembly>> systemPlayerAssemblies = new(UnityPlayerAssemblies.GetSystemAssemblies().ToList);
        public static IReadOnlyList<System.Reflection.Assembly> SystemPlayerAssemblies => systemPlayerAssemblies.Value;

        private static IReadOnlyList<UnityEditor.Compilation.Assembly> GetPlayerAssemblies()
        {
            return CompilationPipeline.GetAssemblies(AssembliesType.Player).Where(IsValid);

            bool IsValid(UnityEditor.Compilation.Assembly x)
            {
                if (x.name.StartsWith("Assembly-CSharp-firstpass"))
                    return false;

                return true;
            }
        }

        public static IEnumerable<Type> GetAllThatImplement(this Type type)
            => type.GetAllSystemAssembliesThatMightImplement()
            .SelectMany((x) => x.GetTypes())
            .Where((x) => x != null && x != type && type.IsAssignableFromGeneric(x));
        public static IEnumerable<System.Reflection.Assembly> GetAllSystemAssembliesThatMightImplement(this Type type)
            => type.GetAllUnityAssembliesThatMightImplement().GetSystemAssemblies();
        public static List<UnityEditor.Compilation.Assembly> GetAllUnityAssembliesThatMightImplement(this Type type)
        {
            var assembly = type.Assembly;
            return UnityPlayerAssemblies.Where(IsValid);

            bool IsValid(UnityEditor.Compilation.Assembly x)
                => assembly.FullName.StartsWith(x.name) || x.HasReferenceTo(assembly);
        }
        public static IEnumerable<System.Reflection.Assembly> GetSystemAssemblies(this IEnumerable<UnityEditor.Compilation.Assembly> assemblies)
        {
            var allSystemAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allSystemAssemblies)
                if (assemblies.Any((x) => assembly.FullName.StartsWith(x.name)))
                    yield return assembly;
        }

        public static Type GetCustomEditor(this Object objectRef)
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(Editor));
            var editorAttributes = assembly.CreateInstance("UnityEditor.CustomEditorAttributes");

            var type = editorAttributes.GetType();
            BindingFlags bf = BindingFlags.Static | BindingFlags.NonPublic;

            MethodInfo findCustomEditorType = type.GetMethod("FindCustomEditorType", bf);
            return (Type)findCustomEditorType.Invoke(editorAttributes, new object[] { objectRef, true });

        }
    }
}
