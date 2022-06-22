using System.Collections.Generic;

namespace UniKit.Types
{
    public interface IReadonlyMap<T1, T2> :
        IEnumerable<KeyValuePair<T1, T2>>
    {
        IReadOnlyDictionary<T1, T2> Forward { get; }
        IReadOnlyDictionary<T2, T1> Backwards { get; }

        T2 this[T1 key] { get; }
        T1 this[T2 key] { get; }

        bool TryGetValue(T1 key, out T2 value);
        bool TryGetValue(T2 key, out T1 value);

        bool ContainsKey(T1 key);
        bool ContainsKey(T2 key);

        bool Contains(KeyValuePair<T1, T2> item);
        bool Contains(KeyValuePair<T2, T1> item);
    }

    public interface IMap<T1, T2> :
        IReadonlyMap<T1, T2>
    {
        new T2 this[T1 key] { get; set; }
        new T1 this[T2 key] { get; set; }

        void Add(KeyValuePair<T1, T2> item);
        void Add(T1 key, T2 value);
        void Add(KeyValuePair<T2, T1> item);
        void Add(T2 key, T1 value);

        bool Remove(T1 key);
        bool Remove(KeyValuePair<T1, T2> item);
        bool Remove(T2 key);
        bool Remove(KeyValuePair<T2, T1> item);

        void Clear();
    }
}
