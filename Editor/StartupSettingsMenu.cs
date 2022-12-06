using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniKit
{
    static class StartupSettingsMenu
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/Startup Scripts", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Startup Scripts",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = GuiHandler,

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(Keywords())

            };

            return provider;

            IEnumerable<string> Keywords()
            {
                yield return "Startup";
            }
        }

        private static void GuiHandler(string searchContext)
        {
            var settings = new SerializedObject(StartupSettings.Data);
            EditorGUILayout.PropertyField(settings.FindProperty("startupScripts"), new GUIContent("Startup Scripts"));
            //EditorGUILayout.PropertyField(settings.FindProperty("startupScriptsCollection"), new GUIContent("Startup Scripts Collection"));
            settings.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}