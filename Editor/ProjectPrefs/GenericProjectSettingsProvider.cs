using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniKit.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniKit.Editor
{
    public class GenericProjectSettingsProvider : MonoBehaviour
    {
        internal static IEnumerable<Type> settingsTypes = GetGenericSettingsTypes();
        internal static BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        [SettingsProviderGroup]
        internal static SettingsProvider[] FetchGenericSettingsProviderList()
        {
            List<SettingsProvider> providers = new List<SettingsProvider>();

            foreach (Type settingsType in settingsTypes)
            {
                if (!settingsType.TryFindProperty("Data", bindingFlags, out var property))
                    continue;

                var serielizedObject = LoadSerializedObject();
                if (serielizedObject == null || serielizedObject.targetObject == null)
                    continue;

                var provider = new SettingsProvider($"Project/{Application.productName.ToDisplayName()}/{settingsType.Name.ToDisplayName()}", SettingsScope.Project)
                {
                    guiHandler = (x) => Gui(settingsType, x)
                };

                providers.Add(provider);
                void Gui(Type type, string obj)
                {
                    if (serielizedObject == null || serielizedObject.targetObject == null)
                    {
                        serielizedObject = LoadSerializedObject();
                        if (serielizedObject == null || serielizedObject.targetObject == null)
                            return;
                    }
                    var iterator = serielizedObject.GetIterator();
                    GUI.enabled = false;
                    //Draw first with gui disabled because it's always the Monoscript field
                    iterator.NextVisible(true);
                    EditorGUILayout.PropertyField(iterator);
                    GUI.enabled = true;

                    bool enterChildren = true;
                    while (iterator.NextVisible(enterChildren))
                    {
                        enterChildren = false;
                        EditorGUILayout.PropertyField(iterator);
                    }

                    serielizedObject.ApplyModifiedProperties();
                }

                SerializedObject LoadSerializedObject()
                {
                    var value = property.GetValue(null) as Object;
                    if (value == null)
                        return null;

                    return new SerializedObject(value);
                }
            }

            return providers.ToArray();
        }

        private static IEnumerable<Type> GetGenericSettingsTypes()
        {
            var type = typeof(GenericProjectSettings<>);
            var assemblies = type
                .GetAllThatMightImplement()
                .GetSystemAssemblies()
                .ToList();

            assemblies.RemoveAll((x) => x.FullName.StartsWith("Assembly-CSharp-firstpass"));

            if (assemblies == null || assemblies.Count == 0)
                return null;

            var allTypes = assemblies.SelectMany((x) => x.GetTypes()).ToArray();
            return allTypes.Where((x) => x != null && x != type && type.IsAssignableFromGeneric(x));
        }
    }
}
