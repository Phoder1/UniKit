using System;
using UnityEngine;

namespace Phoder1.Core
{
    public class LazyGameObject<T>
        where T : Component
    {
        public readonly Lazy<T> Lazy;
        public T Value => Lazy.Value;
        public LazyGameObject(T prefab, Transform parent = null, bool dontDestroyOnLoad = false)
        {
            Lazy = new Lazy<T>(Instantiate);

            T Instantiate()
            {
                var newComp = UnityEngine.Object.Instantiate(prefab, parent);

                if (dontDestroyOnLoad)
                    UnityEngine.Object.DontDestroyOnLoad(newComp);

                return newComp;
            }
        }

    }
}
