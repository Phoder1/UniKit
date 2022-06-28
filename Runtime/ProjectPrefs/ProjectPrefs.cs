using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UniKit.Project
{
    public class ProjectPrefs : GenericProjectSettings<ProjectPrefs>
    {
        [SerializeField]
        private PrefsProperty<int>[] intProperties;
        [SerializeField]
        private PrefsProperty<string>[] stringProperties;
        [SerializeField]
        private PrefsProperty<float>[] floatProperties;
        [SerializeField]
        private PrefsProperty<GameObject>[] gameObjectProperties;
        [SerializeField]
        private PrefsProperty<Sprite>[] spriteProperties;


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

        public override string MenuPath => $"UniKit/{typeof(ProjectPrefs).Name.ToDisplayName()}";
        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static float GetFloat(string key)
            => Array.Find(Data.floatProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static int GetInt(string key)
            => Array.Find(Data.intProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static string GetString(string key)
            => Array.Find(Data.stringProperties, (x) => x.Name == key).Value;
        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static GameObject GetGameObject(string key)
            => Array.Find(Data.gameObjectProperties, (x) => x.Name == key).Value;
        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        public static Sprite GetSprite(string key)
            => Array.Find(Data.spriteProperties, (x) => x.Name == key).Value;

        /// <summary>
        /// Returns true if the given key exists in PlayerPrefs, otherwise returns false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            if (Data == null)
                return false;

            return Data.AllProperties.Any((x) => x.Name == key);
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