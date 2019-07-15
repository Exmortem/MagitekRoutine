using System;
using System.Collections;
using System.Collections.Generic;

namespace Magitek.Utilities.Collections
{
    public class ConcurrentObservableDictionary<TKey, TValue> : ConcurrentObservableBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection
    {
        private readonly Dictionary<TKey, DoubleLinkListIndexNode> _keyToIndex;
        private DoubleLinkListIndexNode _lastNode;

        public ConcurrentObservableDictionary()
        {
            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>();
        }

        public ConcurrentObservableDictionary(IDictionary<TKey, TValue> source)
            : this()
        {

            foreach (KeyValuePair<TKey, TValue> pair in source)
            {
                Add(pair);
            }
        }

        public ConcurrentObservableDictionary(IEqualityComparer<TKey> equalityComparer) : this()
        {
            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(equalityComparer);
        }

        public ConcurrentObservableDictionary(int capactity) : this()
        {
            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(capactity);
        }

        public ConcurrentObservableDictionary(IDictionary<TKey, TValue> source, IEqualityComparer<TKey> equalityComparer) : this(equalityComparer)
        {
            foreach (var pair in source)
            {
                Add(pair);
            }
        }

        public ConcurrentObservableDictionary(int capacity, IEqualityComparer<TKey> equalityComparer) : this()
        {
            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(capacity, equalityComparer);
        }

        public int IndexOfKey(TKey key)
        {
            return DoBaseRead(() => _keyToIndex[key].Index);
        }

        public bool TryGetIndexOf(TKey key, out int index)
        {
            var values = DoBaseRead(() => _keyToIndex.TryGetValue(key, out DoubleLinkListIndexNode node) ? Tuple.Create(true, node.Index) : Tuple.Create(false, 0));
            index = values.Item2;
            return values.Item1;
        }

        public void Add(TKey key, TValue value)
        {
            DoBaseWrite(() => {
                BaseAdd(key, value);
            });
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return DoBaseReadWrite(() => _keyToIndex.ContainsKey(key), () => false, () => {
                BaseAdd(key, value);
                return true;
            });
        }

        public TValue RetrieveOrAdd(TKey key, Func<TValue> getValue)
        {
            var value = default(TValue);
            return DoBaseReadWrite(() => _keyToIndex.ContainsKey(key), () => {
                var index = _keyToIndex[key].Index;
                return WriteCollection[index].Value;
            }, () => {
                value = getValue();
            }, () => {
                BaseAdd(key, value);
                return value;
            });
        }

        protected virtual void BaseAdd(TKey key, TValue value)
        {
            var node = new DoubleLinkListIndexNode(_lastNode, _keyToIndex.Count);
            _keyToIndex.Add(key, node);
            _lastNode = node;
            WriteCollection.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            return DoBaseRead(() => _keyToIndex.ContainsKey(key));
        }

        public ICollection<TKey> Keys => new ConcurrentKeyCollection<TKey, TValue>(this);

        public bool Remove(TKey key)
        {
            return DoBaseWrite(() => {
                if (!_keyToIndex.TryGetValue(key, out DoubleLinkListIndexNode node))
                    return _keyToIndex.Remove(key);

                WriteCollection.RemoveAt(node.Index);
                if (node == _lastNode)
                {
                    _lastNode = node.Previous;
                }
                node.Remove();
                return _keyToIndex.Remove(key);
            });
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var values = DoBaseRead(() => _keyToIndex.TryGetValue(key, out DoubleLinkListIndexNode index) ? Tuple.Create(true, WriteCollection[index.Index].Value) : Tuple.Create(false, default(TValue)));
            value = values.Item2;
            return values.Item1;
        }

        public ICollection<TValue> Values => new ConcurrentValueCollection<TKey, TValue>(this);

        public TValue this[TKey key]
        {
            get
            {
                return DoBaseRead(() => {
                    var index = _keyToIndex[key].Index;
                    return WriteCollection[index].Value;
                });
            }
            set
            {
                DoBaseWrite(() => {
                    if (_keyToIndex.ContainsKey(key))
                    {
                        var index = _keyToIndex[key].Index;
                        WriteCollection[index] = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                    {
                        BaseAdd(key, value);
                    }
                });
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            DoBaseClear(() => { _keyToIndex.Clear(); _lastNode = null; });
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return DoBaseRead(() => ReadCollection.Contains(item));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            DoBaseRead(() => { ReadCollection.CopyTo(array, arrayIndex); });
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        public int Count
        {
            get
            {
                return DoBaseRead(() => ReadCollection.Count);
            }
        }

        public bool IsReadOnly => false;

        public void CopyTo(Array array, int index)
        {
            DoBaseRead(() => { ((ICollection)ReadCollection).CopyTo(array, index); });
        }

        public bool IsSynchronized
        {
            get
            {
                return DoBaseRead(() => ((ICollection)ReadCollection).IsSynchronized);
            }
        }

        public object SyncRoot
        {
            get
            {
                return DoBaseRead(() => ((ICollection)ReadCollection).SyncRoot);
            }
        }

    }
}
