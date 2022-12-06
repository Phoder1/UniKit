using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    [Serializable]
    public class ScriptCollection<T> : IReadOnlyList<T>, IList<T>
        where T : ScriptableObject
    {
        [SerializeField]
        private List<T> _scripts;



        T IList<T>.this[int index] { get => _scripts[index]; set => _scripts[index] = value; }

        public int Count                                => _scripts.Count;
        public T this[int index]                        => _scripts[index];
        public IEnumerator<T> GetEnumerator()           => _scripts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()         => _scripts.GetEnumerator();
        public int IndexOf(T item)                      => _scripts.IndexOf(item);
        public void Insert(int index, T item)           => _scripts.Insert(index, item);
        public void RemoveAt(int index)                 => _scripts.RemoveAt(index);
        public void Add(T item)                         => _scripts.Add(item);
        public void Clear()                             => _scripts.Clear();
        public bool Contains(T item)                    => _scripts.Contains(item);
        public void CopyTo(T[] array, int arrayIndex)   => _scripts.CopyTo(array, arrayIndex);
        public bool Remove(T item)                      => _scripts.Remove(item);

        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();
    }
}
