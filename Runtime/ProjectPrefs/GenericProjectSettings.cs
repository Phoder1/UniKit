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
    public abstract class GenericProjectSettings<TChild> : ScriptableObject
        where TChild : GenericProjectSettings<TChild>
    {
        private static string ResourcePath => typeof(TChild).Name;
        private static string AssetPath => Path.ChangeExtension(Path.Combine("Assets/Resources", ResourcePath), "asset");
        public virtual string MenuPath => ResourcePath;

        private static GenericProjectSettings<TChild> data;
        public static GenericProjectSettings<TChild> Data
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
            Data = Resources.Load(ResourcePath) as GenericProjectSettings<TChild>;
        }
#if UNITY_EDITOR
        private static GenericProjectSettings<TChild> GetOrCreateSettings()
        {
            if (Application.isPlaying)
                return data;

            GenericProjectSettings<TChild> settings = AssetDatabase.LoadAssetAtPath<GenericProjectSettings<TChild>>(AssetPath);

            if (settings == null)
                settings = CreateSettings(settings);

            return settings;
        }

        private static GenericProjectSettings<TChild> CreateSettings(GenericProjectSettings<TChild> settings)
        {
            settings = CreateInstance<TChild>();
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
