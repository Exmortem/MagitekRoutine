using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Collections
{
    public class ConcurrentKeyCollection<TKey, TValue> : ImmutableCollectionBase<TKey>
    {
        private readonly ICollection<KeyValuePair<TKey, TValue>> _pairs;

        public ConcurrentKeyCollection(ConcurrentObservableDictionary<TKey, TValue> dictionary)
        {
            _pairs = dictionary.Snapshot;
        }

        public override int Count => _pairs.Count;

        public override bool Contains(TKey item)
        {
            return _pairs.Any(pair => item.Equals(pair.Key));
        }

        public override void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw (new System.ArgumentNullException());
            }

            foreach (var pair in _pairs)
            {
                array[arrayIndex] = pair.Key;
                ++arrayIndex;
            }
        }

        public override IEnumerator<TKey> GetEnumerator()
        {
            return _pairs.Select(pair => pair.Key).GetEnumerator();
        }
    }
}
