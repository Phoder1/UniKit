using System;
using UnityEditor;
using UnityEngine;

namespace UniKit.Project
{
    public class ProjectPrefs : ScriptableObject
    {
        public const string ProjectPrefAssetPath = "Assets/Resources/ProjectPrefAsset.asset";

        [SerializeField]
        private PrefsSettings prefsSettings = new PrefsSettings();

        private static readonly Lazy<ProjectPrefs> prefs = new Lazy<ProjectPrefs>(GetOrCreateSettings);
        public static ProjectPrefs Prefs => prefs.Value;
        private static ProjectPrefs GetOrCreateSettings()
        {
            ProjectPrefs settings = GetSettings();

            if (settings == null)
                settings = CreateSettings(settings);

            return settings;
        }

        private static ProjectPrefs CreateSettings(ProjectPrefs settings)
        {
            settings = default;
#if UNITY_EDITOR
            settings = CreateInstance<ProjectPrefs>();
            AssetDatabase.CreateAsset(settings, ProjectPrefAssetPath);
            AssetDatabase.SaveAssets();
#endif
            return settings;
        }

        private static ProjectPrefs GetSettings()
        {
            ProjectPrefs settings;
#if UNITY_EDITOR
            if (Application.isPlaying)
                settings = Resources.Load<ProjectPrefs>(ProjectPrefAssetPath);
            else
                settings = AssetDatabase.LoadAssetAtPath<ProjectPrefs>(ProjectPrefAssetPath);
#else
            settings = Resources.Load<ProjectPrefs>(ProjectPrefAssetPath);
#endif

            return settings;
        }

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static float GetFloat(string key)
            => Array.Find(GetOrCreateSettings().prefsSettings.floatProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static int GetInt(string key)
            => Array.Find(GetOrCreateSettings().prefsSettings.intProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static string GetString(string key)
            => Array.Find(GetOrCreateSettings().prefsSettings.stringProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns true if the given key exists in PlayerPrefs, otherwise returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
            => Array.FindIndex(GetOrCreateSettings().prefsSettings.stringProperties, (x) => x.Name == key) != -1;

        [Serializable]
        private class PrefsSettings
        {
            [SerializeField]
            public PrefsProperty<int>[] intProperties;
            [SerializeField]
            public PrefsProperty<string>[] stringProperties;
            [SerializeField]
            public PrefsProperty<float>[] floatProperties;

            [Serializable]
            public class PrefsProperty<T>
            {
                [SerializeField]
                private string name;
                [SerializeField]
                private T value;

                public string Name => name;
                public T Value => value;
            }
        }
    }
}