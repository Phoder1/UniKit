using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniKit
{
    public class SingletonScriptableObject<TChild> : ScriptableObject
                where TChild : SingletonScriptableObject<TChild>
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
            preloaded.RemoveAll((x) => x == null);
            PlayerSettings.SetPreloadedAssets(preloaded.ToArray());
            return settings;
        }
#endif
    }
}
