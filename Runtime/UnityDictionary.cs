using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.Core
{
    public class UnityDictionary<TKey, TValue> : MonoBehaviour, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<KeyValuePair> _bones;
        public TValue this[TKey key]
        {
            get => TryGetValue(key, out var val) ? val : throw new KeyNotFoundException();
            set
            {
                if (!ContainsKey(key))
                    throw new KeyNotFoundException();

                _bones.Find((x) => Equals(x.Key, key)).Transform = value;
            }
        }

        public ICollection<TKey> Keys => _bones.ConvertAll((x) => x.Key);

        public ICollection<TValue> Values => _bones.ConvertAll((x) => x.Transform);

        public int Count => _bones.Count;

        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new Exception("An element with the same key already exists in the Dictionary.");

            _bones.Add(new KeyValuePair(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
                throw new Exception("An element with the same key already exists in the Dictionary.");

            _bones.Add(item);
        }

        public void Clear()
            => _bones.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => _bones.Find((x) => Equals(x.Key, item.Key) && Equals(x.Transform, item.Value)) != null;

        public bool ContainsKey(TKey key)
            => _bones.Find((x) => Equals(x.Key, key)) != null;
        public bool TryGetValue(TKey key, out TValue value)
        {
            var pair = _bones.Find((x) => Equals(x.Key, key));
            if (pair == null)
            {
                value = default;
                return false;
            }

            value = pair.Transform;
            return true;
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }


        public bool Remove(TKey key)
        {
            int index = _bones.FindIndex((x) => Equals(x.Key, key));

            if (index > -1)
            {
                _bones.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = _bones.FindIndex((x) => Equals(x.Key, item.Key));

            if (index > -1)
            {
                _bones.RemoveAt(index);
                return true;
            }

            return false;
        }



        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in _bones)
                yield return pair;
        }

        [Serializable]
        private class KeyValuePair
        {
            [SerializeField]
            private TKey _boneID;
            [SerializeField]
            private TValue _value;

            public KeyValuePair(TKey boneID, TValue transform)
            {
                _boneID = boneID ?? throw new ArgumentNullException(nameof(boneID));
                Transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            public TKey Key => _boneID;
            public TValue Transform { get => _value; set => _value = value; }

            public static implicit operator KeyValuePair(KeyValuePair<TKey, TValue> pair)
                => new KeyValuePair(pair.Key, pair.Value);

            public static implicit operator KeyValuePair<TKey, TValue>(KeyValuePair pair)
                => new KeyValuePair<TKey, TValue>(pair.Key, pair.Transform);
        }
    }
}
