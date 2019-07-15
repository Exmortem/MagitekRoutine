using System;
using System.Collections;
using System.Collections.Generic;

namespace Magitek.Utilities.Collections
{
    public abstract class ImmutableCollectionBase<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        public abstract int Count { get; }

        public bool IsReadOnly => true;

        public void Add(T item)
        {
            throw (new NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
        }

        public void Clear()
        {
            throw (new NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
        }

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public bool Remove(T item)
        {
            throw (new NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
        }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();
    }
}
