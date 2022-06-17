using System;
using System.Collections.Concurrent;

namespace Phoder1.Patterns
{
    public interface IPoolable
    {
        bool Valid { get; }
        void Discarded();
        void Drawn();
    }

    public interface IObjectPool<T> where T : IPoolable
    {
        void Discard(T item);
        T Draw();
    }

    public class ObjectPool<T> : IObjectPool<T> where T : IPoolable
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _constructor;
        public ObjectPool(Func<T> constructor)
        {
            _constructor = constructor ?? throw new NullReferenceException();
            _objects = new ConcurrentBag<T>();
        }

        public T Draw()
        {
            while (!_objects.IsEmpty)
            {
                if (_objects.TryTake(out T item) && item != null && item.Valid)
                {
                    item.Drawn();
                    return item;
                }
            }
            var newItem = _constructor();
            newItem.Drawn();
            return newItem;
        }

        public void Discard(T item)
        {
            item.Discarded();
            _objects.Add(item);
        }
    }
}
