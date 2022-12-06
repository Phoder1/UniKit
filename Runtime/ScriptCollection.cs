using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    [Serializable]
    public class ScriptCollection<T>
    where T : ScriptableObject
    {
        [SerializeField]
        private T[] _scripts;

        public IReadOnlyList<T> Scripts;
    }
}
