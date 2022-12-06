#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
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
    public static class GenericProjectSettingsProvider
    {

        internal static IReadOnlyList<Type> settingsTypes = typeof(GenericProjectSettings<>).GetAllThatImplement().ToArray();
        internal static BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        [SettingsProviderGroup]
        internal static SettingsProvider[] FetchGenericSettingsProviderList()
        {
            List<SettingsProvider> providers = new List<SettingsProvider>(); 

            foreach (Type settingsType in settingsTypes)
            {
                if (!settingsType.TryFindProperty("Data", bindingFlags, out var property))
                    continue;

                var editor = LoadEditor(out var value);
                if (editor == null || editor.target == null)
                    continue;

                string menuPath;
                if (settingsType.TryFindProperty("MenuPath", out var menuPathProp))
                    menuPath = (string)menuPathProp.GetValue(value);
                else
                    menuPath = DefaultMenuPath(settingsType);

                var provider = new SettingsProvider($"Project/{menuPath}", SettingsScope.Project)
                {
                    guiHandler = (x) => Gui(settingsType, x)
                };

                providers.Add(provider);
                void Gui(Type type, string obj)
                {
                    if (editor == null || editor.target == null)
                    {
                        editor = LoadEditor(out value);
                        if (editor == null || editor.target == null)
                            return;
                    }
                    //Todo: support custom inspectors
                    editor.OnInspectorGUI();
                }

                UnityEditor.Editor LoadEditor(out Object value)
                {
                    value = property.GetValue(null) as Object;
                    if (value == null)
                        return null;

                    var customEditorType = value.GetType().GetCustomEditorTypes()?.FirstOrDefault();


                    return UnityEditor.Editor.CreateEditor(value, customEditorType);
                }
            }

            return providers.ToArray();
        }
        public static string DefaultMenuPath(Type type)
            => $"{Application.productName.ToDisplayName()}/{type.Name.ToDisplayName()}";
    }
}
