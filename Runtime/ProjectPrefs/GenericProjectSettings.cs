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
        private static string AssetPath 
            => Path.ChangeExtension(Path.Combine("Assets/Resources", ResourcePath), "asset");
        public virtual string MenuPath 
            => $"{Application.productName.ToDisplayName()}/{typeof(TChild).Name.ToDisplayName()}";

        private static TChild data;
        public static TChild Data
        {
            get
            {
                if (data == null)
                {
                    if (Application.isPlaying)
                        LoadSettings();
#if UNITY_EDITOR
                    else
                        data = GetOrCreateSettings();
#endif
                }
                return data;
            }
            private set => data = value;
        }

        public static void LoadSettings()
        {
            Data = Resources.Load(ResourcePath) as TChild;
        }
#if UNITY_EDITOR
        private static TChild GetOrCreateSettings()
        {
            if (Application.isPlaying)
                return data;

            TChild settings = AssetDatabase.LoadAssetAtPath<TChild>(AssetPath);

            if (settings == null)
                settings = CreateSettings(settings);

            return settings;
        }

        private static TChild CreateSettings(TChild settings)
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
