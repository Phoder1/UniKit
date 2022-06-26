using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Object = UnityEngine.Object;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniKit
{
    public abstract class GenericProjectSettings<T, TChild> : ScriptableObject
        where TChild : GenericProjectSettings<T, TChild>
        where T : struct
    {
        [SerializeField]
        private T settings;

        private static string ResourcePath => nameof(TChild);
        private static string AssetPath => Path.ChangeExtension(Path.Combine("Assets/Resources", ResourcePath), "asset");

        private static GenericProjectSettings<T, TChild> data;
        public static GenericProjectSettings<T, TChild> Data
        {
            get
            {
#if UNITY_EDITOR
                if (data == null)
                {
                    if (Application.isPlaying)
                        LoadSettings();
                    else
                        data = GetOrCreateSettings();
                }
#endif
                return data;
            }
            private set => data = value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void LoadPrefs()
        {
            if (Application.isPlaying)
                LoadSettings();
            else
                Data = GetOrCreateSettings();
        }

        public static void LoadSettings()
        {
            Data = Resources.Load(ResourcePath) as GenericProjectSettings<T, TChild>;
        }
#if UNITY_EDITOR
        private static GenericProjectSettings<T, TChild> GetOrCreateSettings()
        {
            if (Application.isPlaying)
                return data;

            GenericProjectSettings<T, TChild> settings = AssetDatabase.LoadAssetAtPath<GenericProjectSettings<T, TChild>>(AssetPath);

            if (settings == null)
                settings = CreateSettings(settings);

            return settings;
        }

        private static GenericProjectSettings<T, TChild> CreateSettings(GenericProjectSettings<T, TChild> settings)
        {
            settings = CreateInstance<GenericProjectSettings<T, TChild>>();
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();

            List<Object> preloaded = new List<Object>(PlayerSettings.GetPreloadedAssets());
            preloaded.Add(settings);
            PlayerSettings.SetPreloadedAssets(preloaded.ToArray());
            return settings;
        }
#endif
    }
}
