using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
#if !UniRxLibrary
using UnityEngine;
#endif

namespace Phoder1.Core.Types
{
    public interface IReadonlyReactiveMap<T1, T2> :
        IReadonlyMap<T1, T2>
    {
        IObservable<DictionaryAddEvent<T1, T2>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<DictionaryRemoveEvent<T1, T2>> ObserveRemove();
        IObservable<DictionaryReplaceEvent<T1, T2>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }

    public interface IReactiveMap<T1, T2> :
        IReadonlyReactiveMap<T1, T2>,
        IMap<T1, T2>
    {

    }
    [Serializable]
    public class ReactiveMap<T1, T2> : IReactiveMap<T1, T2>
    {

#if !UniRxLibrary
        [SerializeField]
#endif
        private readonly Map<T1, T2> inner = new Map<T1, T2>();

        [NonSerialized]
        private Lazy<Subject<DictionaryAddEvent<T1, T2>>> addSubject = new Lazy<Subject<DictionaryAddEvent<T1, T2>>>();
        public IObservable<DictionaryAddEvent<T1, T2>> ObserveAdd() => addSubject.Value;

        [NonSerialized]
        private Lazy<Subject<int>> countSubject = new Lazy<Subject<int>>();
        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false) => countSubject.Value;

        [NonSerialized]
        private Lazy<Subject<DictionaryRemoveEvent<T1, T2>>> removeSubject = new Lazy<Subject<DictionaryRemoveEvent<T1, T2>>>();
        public IObservable<DictionaryRemoveEvent<T1, T2>> ObserveRemove() => removeSubject.Value;

        [NonSerialized]
        private Lazy<Subject<Unit>> resetSubject = new Lazy<Subject<Unit>>();
        public IObservable<Unit> ObserveReset() => resetSubject.Value;

        [NonSerialized]
        private Lazy<Subject<DictionaryReplaceEvent<T1, T2>>> replaceSubject = new Lazy<Subject<DictionaryReplaceEvent<T1, T2>>>();
        public IObservable<DictionaryReplaceEvent<T1, T2>> ObserveReplace() => replaceSubject.Value;

        public T2 this[T1 key]
        {
            get => inner[key];
            set => inner[key] = value;
        }
        public T1 this[T2 key]
        {
            get => inner[key];
            set
            {
                if (TryGetValue(key, out var oldValue))
                {
                    inner[key] = value;
                    if (dictionaryReplace != null) dictionaryReplace.OnNext(new DictionaryReplaceEvent<TKey, TValue>(key, oldValue, value));
                }
                else
                {
                    inner[key] = value;
                    if (dictionaryAdd != null) dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
            }
        }


        public void Add(KeyValuePair<T1, T2> item)
        {
            ((IMap<T1, T2>)inner).Add(item);
        }

        public void Add(T1 key, T2 value)
        {
            ((IMap<T1, T2>)inner).Add(key, value);
        }

        public void Add(KeyValuePair<T2, T1> item)
        {
            ((IMap<T1, T2>)inner).Add(item);
        }

        public void Add(T2 key, T1 value)
        {
            ((IMap<T1, T2>)inner).Add(key, value);
        }

        public bool Remove(T1 key)
        {
            return ((IMap<T1, T2>)inner).Remove(key);
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            return ((IMap<T1, T2>)inner).Remove(item);
        }

        public bool Remove(T2 key)
        {
            return ((IMap<T1, T2>)inner).Remove(key);
        }

        public bool Remove(KeyValuePair<T2, T1> item)
        {
            return ((IMap<T1, T2>)inner).Remove(item);
        }

        public void Clear()
        {
            ((IMap<T1, T2>)inner).Clear();
        }

        #region Normal map
        public IReadOnlyDictionary<T1, T2> Forward => ((IReadonlyMap<T1, T2>)inner).Forward;
        public IReadOnlyDictionary<T2, T1> Backwards => ((IReadonlyMap<T1, T2>)inner).Backwards;

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return ((IReadonlyMap<T1, T2>)inner).Contains(item);
        }

        public bool Contains(KeyValuePair<T2, T1> item)
        {
            return ((IReadonlyMap<T1, T2>)inner).Contains(item);
        }

        public bool ContainsKey(T1 key)
        {
            return ((IReadonlyMap<T1, T2>)inner).ContainsKey(key);
        }

        public bool ContainsKey(T2 key)
        {
            return ((IReadonlyMap<T1, T2>)inner).ContainsKey(key);
        }
        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T1, T2>>)inner).GetEnumerator();
        }
        public bool TryGetValue(T1 key, out T2 value)
        {
            return ((IReadonlyMap<T1, T2>)inner).TryGetValue(key, out value);
        }

        public bool TryGetValue(T2 key, out T1 value)
        {
            return ((IReadonlyMap<T1, T2>)inner).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => inner.GetEnumerator();
        #endregion


    }
}
