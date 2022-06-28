using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;
using UniKit.Reflection;

namespace UniKit.Editor
{
    public static class EditorExtensionMethods
    {
        public static void FlatProperty(this SerializedProperty property, GUIContent guiContent = null)
        {
            if (!property.hasVisibleChildren)
            {
                EditorGUILayout.PropertyField(property);
                return;
            }

            var childs = property.GetChilds();

            int childCount = 0;
            foreach (var child in childs)
            {
                childCount++;
                EditorGUILayout.PropertyField(child, guiContent);
            }

            if (childCount == 0)
                EditorGUILayout.PropertyField(property);
        }

        public static IEnumerable<SerializedProperty> GetChilds(this SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);

            while (true)
            {
                if (SerializedProperty.EqualContents(property, nextElement))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }
        public static bool GetIsArrayElement(this SerializedProperty property)
            => property.propertyPath.Contains('[') && property.propertyPath.Contains(']');
        public static int GetArrayPropertyIndex(this SerializedProperty property)
            => int.Parse(property.propertyPath.Split('[').LastOrDefault().TrimEnd(']'));
        public static T GetTarget<T>(this PropertyDrawer drawer, SerializedProperty property)
        {
            T myDataClass = (T)drawer.fieldInfo.GetValue(property.serializedObject.targetObject);
            return myDataClass;
        }
        public static object GetTarget(this PropertyDrawer drawer, SerializedProperty property)
        {
            object myDataClass = drawer.fieldInfo.GetValue(property.serializedObject.targetObject);
            return myDataClass;
        }

        public static FieldInfo GetField(this SerializedProperty property)
        {
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            Object targetObj = property.serializedObject.targetObject;

            FieldInfo field = null;

            string[] name = property.propertyPath.Split('.');
            Type type = targetObj.GetType();

            for (int i = 0; i < name.Length; i++)
            {
                field = type.GetField(name[i], bindings);
                type = field.FieldType;
            }

            return field;
        }

        public static void AsRefField(this ref int value, string name = null, bool guiEnabled = true)
            => value = value.AsField(name, guiEnabled);
        public static int AsField(this int value, string name = null, bool guiEnabled = true)
        {
            bool guiTemp = GUI.enabled;
            GUI.enabled = guiEnabled;

            int temp;

            if (name == null)
                temp = EditorGUILayout.IntField(value);
            else
                temp = EditorGUILayout.IntField(name, value);

            GUI.enabled = guiTemp;

            return temp;
        }
        public static Object AsField(this Object value, string name = null, bool guiEnabled = true)
        {
            bool guiTemp = GUI.enabled;
            GUI.enabled = guiEnabled;

            Object temp;

            if (name == null)
                temp = EditorGUILayout.ObjectField(value, value.GetType(), true);
            else
                temp = EditorGUILayout.ObjectField(name, value, value.GetType(), true);

            GUI.enabled = guiTemp;

            return temp;
        }
        public static void AsHelpBox(this string value, MessageType type = MessageType.None, bool wide = true)
        {
            EditorGUILayout.HelpBox(value, type, wide);
        }
        public static string AsField(this string value, string name = null, bool guiEnabled = true)
        {
            bool guiTemp = GUI.enabled;
            GUI.enabled = guiEnabled;

            string temp;

            if (name == null)
                temp = EditorGUILayout.TextField(value);
            else
                temp = EditorGUILayout.TextField(name, value);

            GUI.enabled = guiTemp;

            return temp;
        }
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, UnityEditor.Compilation.Assembly referencedAssembly)
        => assembly.HasReferenceTo(referencedAssembly.name);
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, System.Reflection.Assembly referencedAssembly)
            => assembly.HasReferenceTo(referencedAssembly.FullName);
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, string referencedAssembly)
            => Array.Exists(assembly.assemblyReferences, (x) => referencedAssembly.StartsWith(x.name));
        public static IEnumerable<Type> GetAllThatImplement(this Type type)
            => type.GetAllSystemAssembliesThatMightImplement()
            .SelectMany((x) => x.GetTypes())
            .Where((x) => x != null && x != type && type.IsAssignableFromGeneric(x));
        public static IEnumerable<System.Reflection.Assembly> GetAllSystemAssembliesThatMightImplement(this Type type)
            => type.GetAllUnityAssembliesThatMightImplement().GetSystemAssemblies();
        public static List<UnityEditor.Compilation.Assembly> GetAllUnityAssembliesThatMightImplement(this Type type)
        {
            var assembly = type.Assembly;
            var allAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            var assemblies = allAssemblies.Where(IsValid);
            return assemblies;

            bool IsValid(UnityEditor.Compilation.Assembly x)
            {
                if (x.name.StartsWith("Assembly-CSharp-firstpass")) 
                    return false;

                if (!assembly.FullName.StartsWith(x.name) && !x.HasReferenceTo(assembly))
                    return false;

                return true;
            }
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
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var editorAttributes = assembly.CreateInstance("UnityEditor.CustomEditorAttributes");

            var type = editorAttributes.GetType();
            BindingFlags bf = BindingFlags.Static | BindingFlags.NonPublic;

            MethodInfo findCustomEditorType = type.GetMethod("FindCustomEditorType", bf);
            return (Type)findCustomEditorType.Invoke(editorAttributes, new object[] { objectRef, true });

        }
    }
}