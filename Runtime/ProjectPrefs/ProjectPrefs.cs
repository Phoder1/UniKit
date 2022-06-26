using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace UniKit.Project
{
    public class ProjectPrefs : ScriptableObject
    {
        public const string PPResourcesPath = "ProjectPrefAsset";
        public const string PPAssetPath = "Assets/Resources/ProjectPrefAsset.asset";

        [SerializeField]
        private PrefsSettings prefsSettings = new PrefsSettings();

        private static ProjectPrefs prefs;
        public static ProjectPrefs Prefs
        {
            get
            {
#if UNITY_EDITOR
                if (prefs == null)
                {
                    if (Application.isPlaying)
                        LoadSettings();
                    else
                        prefs = GetOrCreateSettings();
                }
#endif
                return prefs;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void LoadPrefs()
        {
            if (Application.isPlaying)
                LoadSettings();
            else
                prefs = GetOrCreateSettings();
        }

        public static void LoadSettings()
        {
            prefs = Resources.Load(PPResourcesPath) as ProjectPrefs;
        }
#if UNITY_EDITOR
        private static ProjectPrefs GetOrCreateSettings()
        {
            if (Application.isPlaying)
                return prefs;

            ProjectPrefs settings = AssetDatabase.LoadAssetAtPath<ProjectPrefs>(PPAssetPath); 

            if (settings == null)
                settings = CreateSettings(settings);

            return settings;
        }

        private static ProjectPrefs CreateSettings(ProjectPrefs settings)
        {
            settings = CreateInstance<ProjectPrefs>();
            AssetDatabase.CreateAsset(settings, PPAssetPath);
            AssetDatabase.SaveAssets();

            List<Object> preloaded = new List<Object>(PlayerSettings.GetPreloadedAssets());
            preloaded.Add(settings);
            PlayerSettings.SetPreloadedAssets(preloaded.ToArray());
            return settings;
        }
#endif

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static float GetFloat(string key)
            => Array.Find(Prefs.prefsSettings.floatProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static int GetInt(string key)
            => Array.Find(Prefs.prefsSettings.intProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static string GetString(string key)
            => Array.Find(Prefs.prefsSettings.stringProperties, (x) => x.Name == key).Value;
        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static GameObject GetGameObject(string key)
            => Array.Find(Prefs.prefsSettings.gameObjectProperties, (x) => x.Name == key).Value;
        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static Sprite GetSprite(string key)
            => Array.Find(Prefs.prefsSettings.spriteProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns true if the given key exists in PlayerPrefs, otherwise returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            if (Prefs == null)
                return false;

            return Prefs.prefsSettings.AllProperties.Any((x) => x.Name == key);
        }

        [Serializable]
        private class PrefsSettings
        {
            [SerializeField]
            public PrefsProperty<int>[] intProperties;
            [SerializeField]
            public PrefsProperty<string>[] stringProperties;
            [SerializeField]
            public PrefsProperty<float>[] floatProperties;
            [SerializeField]
            public PrefsProperty<GameObject>[] gameObjectProperties;
            [SerializeField]
            public PrefsProperty<Sprite>[] spriteProperties;


            public IEnumerable<IPrefsProperty> AllProperties
            {
                get
                {
                    foreach (var prop in intProperties)
                        yield return prop;
                    foreach (var prop in stringProperties)
                        yield return prop;
                    foreach (var prop in floatProperties)
                        yield return prop;
                    foreach (var prop in gameObjectProperties)
                        yield return prop;
                    foreach (var prop in spriteProperties)
                        yield return prop;
                }
            }


            public interface IPrefsProperty
            {
                string Name { get; }
                object value { get; }
            }
            [Serializable]
            public class PrefsProperty<T> : IPrefsProperty
            {
                [SerializeField]
                private string name;
                [SerializeField]
                private T value;

                public string Name => name;
                public T Value => value;
                object IPrefsProperty.value => Value;
            }
        }
    }
}