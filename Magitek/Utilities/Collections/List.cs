using System;
using System.Collections;
using System.Collections.Generic;

namespace Magitek.Utilities.Collections
{
    public class List<T> :
        ConcurrentObservableBase<T>,
        IList<T>,
        ICollection<T>,
        IList,
        ICollection
    {

        public List()
        {
        }

        public List(IEnumerable<T> enumerable) : base(enumerable)
        {
        }

        public int IndexOf(T item)
        {
            return DoBaseRead(() => {
                return ReadCollection.IndexOf(item);
            });
        }

        public void Insert(int index, T item)
        {
            DoBaseWrite(() => {
                WriteCollection.Insert(index, item);
            });
        }

        public void RemoveAt(int index)
        {
            DoBaseWrite(() => {
                WriteCollection.RemoveAt(index);
            });
        }

        public T this[int index]
        {
            get
            {
                return DoBaseRead(() => {
                    return ReadCollection[index];
                });
            }
            set
            {
                DoBaseWrite(() => {
                    WriteCollection[index] = value;
                });
            }
        }

        public void Add(T item)
        {
            DoBaseWrite(() => {
                WriteCollection.Add(item);
            });
        }

        public void Clear()
        {
            DoBaseClear(() => { });
        }

        public bool Contains(T item)
        {
            return DoBaseRead(() => {
                return ReadCollection.Contains(item);
            });
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            DoBaseRead(() => {
                ReadCollection.CopyTo(array, arrayIndex);
            });
        }

        public int Count
        {
            get
            {
                return DoBaseRead(() => {
                    return ReadCollection.Count;
                });
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return DoBaseRead(() => {
                    return ((ICollection<T>)ReadCollection).IsReadOnly;
                });
            }
        }

        public bool Remove(T item)
        {
            return DoBaseWrite(() => {
                return WriteCollection.Remove(item);
            });
        }

        void ICollection.CopyTo(Array array, int index)
        {
            DoBaseRead(() => {
                ((ICollection)ReadCollection).CopyTo(array, index);
            });
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return DoBaseRead(() => {
                    return ((ICollection)ReadCollection).IsSynchronized;
                });
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return DoBaseRead(() => {
                    return ((ICollection)ReadCollection).SyncRoot;
                });
            }
        }

        int IList.Add(object value)
        {
            return DoBaseWrite(() => {
                return ((IList)WriteCollection).Add(value);
            });
        }

        bool IList.Contains(object value)
        {
            return DoBaseRead(() => {
                return ((IList)ReadCollection).Contains(value);
            });
        }

        int IList.IndexOf(object value)
        {
            return DoBaseRead(() => {
                return ((IList)ReadCollection).IndexOf(value);
            });
        }

        void IList.Insert(int index, object value)
        {
            DoBaseWrite(() => {
                ((IList)WriteCollection).Insert(index, value);
            });
        }

        bool IList.IsFixedSize
        {
            get
            {
                return DoBaseRead(() => {
                    return ((IList)ReadCollection).IsFixedSize;
                });
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return DoBaseRead(() => {
                    return ((IList)ReadCollection).IsReadOnly;
                });
            }
        }

        void IList.Remove(object value)
        {
            DoBaseWrite(() => {
                ((IList)WriteCollection).Remove(value);
            });
        }

        void IList.RemoveAt(int index)
        {
            DoBaseWrite(() => {
                ((IList)WriteCollection).RemoveAt(index);
            });
        }

        object IList.this[int index]
        {
            get
            {
                return DoBaseRead(() => {
                    return ((IList)ReadCollection)[index];
                });
            }
            set
            {
                DoBaseWrite(() => {
                    ((IList)WriteCollection)[index] = value;
                });
            }
        }

    }
}
