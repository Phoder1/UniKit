using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniKit
{
    public static class DictionaryExt
    {
        public static Dictionary<TValue, IEnumerable<TKey>> Swap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict)
        {
            var newDict = new Dictionary<TValue, IEnumerable<TKey>>();

            foreach (var pair in dict)
                if (!newDict.ContainsKey(pair.Value))
                    newDict.Add(pair.Value, dict.Where(p => Equals(p.Value, pair.Value)).Select(p => p.Key));

            return newDict;
        }

        public static TValue GetOrInsert<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> newValueFunc)
        {
            if (dict.TryGetValue(key, out var value))
                return value;

            var newValue = newValueFunc();
            dict.Add(key, newValue);

            return newValue;
        }
    }
}
