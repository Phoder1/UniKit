using System.IO;
using UniKit.Project;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniKit.Editor.Project
{
    class ProjectPrefsSettingsProvider : SettingsProvider
    {
        private SerializedObject projectPrefSettings;

        class Styles
        {
            public static GUIContent PrefsSettings = new GUIContent("Prefs Settings");
        }
        public ProjectPrefsSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(ProjectPrefs.ProjectPrefAssetPath);
        }

        // This function is called when the user clicks on the MyCustom element in the Settings window.
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            projectPrefSettings = GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            var property = projectPrefSettings.FindProperty("prefsSettings");
            property.FlatProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new ProjectPrefsSettingsProvider("Project/Project Prefs", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(ProjectPrefs.Prefs);
        }
    }
}