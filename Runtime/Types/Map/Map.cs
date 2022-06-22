using System;
using System.Collections;
using System.Collections.Generic;

namespace UniKit.Types
{
    [Serializable]
    public class Map<T1, T2> : IMap<T1, T2>
    {
        public int Count => _forward.Count;

        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _backwards = new Dictionary<T2, T1>();

        public IReadOnlyDictionary<T1, T2> Forward => _forward;
        public IReadOnlyDictionary<T2, T1> Backwards => _backwards;

        public T2 this[T1 key] { get => _forward[key]; set => _forward[key] = value; }
        public T1 this[T2 key] { get => _backwards[key]; set => _backwards[key] = value; }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return ((IDictionary<T1, T2>)_forward).TryGetValue(key, out value);
        }
        public bool TryGetValue(T2 key, out T1 value)
        {
            return ((IDictionary<T2, T1>)_backwards).TryGetValue(key, out value);
        }

        public ICollection<T1> Keys
            => ((IDictionary<T1, T2>)_forward).Keys;
        public ICollection<T2> Values
            => ((IDictionary<T1, T2>)_forward).Values;

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T1, T2>>)_forward).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forward).GetEnumerator();
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            ((ICollection<KeyValuePair<T1, T2>>)_forward).Add(item);
        }
        public void Add(T1 key, T2 value)
        {
            ((IDictionary<T1, T2>)_forward).Add(key, value);
        }
        public void Add(KeyValuePair<T2, T1> item)
        {
            ((ICollection<KeyValuePair<T2, T1>>)_backwards).Add(item);
        }
        public void Add(T2 key, T1 value)
        {
            ((IDictionary<T2, T1>)_backwards).Add(key, value);
        }

        public bool Remove(T1 key)
        {
            if (!_forward.TryGetValue(key, out var value))
                return false;

            _backwards.Remove(value);
            return _forward.Remove(key);
        }
        public bool Remove(KeyValuePair<T1, T2> item)
        {
            Remove(item.Value);
            return Remove(item.Key);
        }
        public bool Remove(T2 key)
        {
            if (!_backwards.TryGetValue(key, out var value))
                return false;

            _forward.Remove(value);
            return _backwards.Remove(key);
        }
        public bool Remove(KeyValuePair<T2, T1> item)
        {
            Remove(item.Key);
            return Remove(item.Value);
        }

        public void Clear()
        {
            _forward.Clear();
            _backwards.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return ((ICollection<KeyValuePair<T1, T2>>)_forward).Contains(item);
        }
        public bool Contains(KeyValuePair<T2, T1> item)
        {
            return ((ICollection<KeyValuePair<T2, T1>>)_backwards).Contains(item);
        }

        public bool ContainsKey(T1 key)
        {
            return ((IDictionary<T1, T2>)_forward).ContainsKey(key);
        }
        public bool ContainsKey(T2 key)
        {
            return ((IDictionary<T2, T1>)_backwards).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T1, T2>>)_forward).CopyTo(array, arrayIndex);
        }
        public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T2, T1>>)_backwards).CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
            => ((ICollection<KeyValuePair<T1, T2>>)_forward).IsReadOnly;
    }
}
