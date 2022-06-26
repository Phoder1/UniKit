using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniKit.Editor
{
    public class GenericProjectSettingsProvider : MonoBehaviour
    {
        [SettingsProviderGroup]
        internal static SettingsProvider[] FetchGenericSettingsProviderList()
        {
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.GetProperty;
            List<SettingsProvider> providers = new List<SettingsProvider>();
            foreach (Type settingsType in GetGenericSettingsTypes())
            {
                var provider = new SettingsProvider($"Project/{settingsType.Name.ToDisplayName()}", SettingsScope.Project)
                {
                    guiHandler = Gui
                };

                providers.Add(provider);
                void Gui(string obj)
                {
                    var serielizedObject = new SerializedObject(settingsType.GetProperty("Data", bindingFlags).GetValue(null) as Object);
                    var property = serielizedObject.FindProperty("settings");

                    property.FlatProperty();

                    serielizedObject.ApplyModifiedProperties();
                }   
            }

            return providers.ToArray();
        }


        private static IEnumerable<Type> GetGenericSettingsTypes()
        {
            var settingsBaseType = typeof(GenericProjectSettings<,>);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
                if (UserAssembly(assembly))
                    foreach (var type in assembly.GetTypes())
                        if (type != settingsBaseType && settingsBaseType.IsAssignableFrom(type))
                            yield return type;

            bool UserAssembly(Assembly assembly)
            {
                if (assembly.FullName.StartsWith("UnityEngine"))
                    return false;

                if (assembly.FullName.StartsWith("UnityEditor"))
                    return false;

                if (assembly.FullName.StartsWith("System"))
                    return false;

                if (assembly.FullName.StartsWith("Mono."))
                    return false;

                if (assembly.FullName.StartsWith("netstandard"))
                    return false;

                if (assembly.FullName.StartsWith("Assembly-CSharp"))
                    return false;

                if (assembly.FullName.StartsWith("Unity."))
                    return false;

                if (assembly.FullName.StartsWith("Zenject"))
                    return false;

                if (assembly.FullName.StartsWith("Sirenix"))
                    return false;

                if (assembly.FullName.StartsWith("UniRx"))
                    return false;

                if (assembly.FullName.StartsWith("DemiEditor"))
                    return false;

                if (assembly.FullName.StartsWith("Google"))
                    return false;

                if (assembly.FullName.StartsWith("DOTween"))
                    return false;

                if (assembly.FullName.StartsWith("Newtonsoft"))
                    return false;

                return true;
            }
        }
    }
}
