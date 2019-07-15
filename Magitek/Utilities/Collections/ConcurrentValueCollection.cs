using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Collections
{
    public class ConcurrentValueCollection<TKey, TValue> : ImmutableCollectionBase<TValue>
    {

        private readonly ICollection<KeyValuePair<TKey, TValue>> _pairs;

        public ConcurrentValueCollection(ConcurrentObservableDictionary<TKey, TValue> dictionary)
        {
            _pairs = dictionary.Snapshot;
        }

        public override bool Contains(TValue item)
        {
            return _pairs.Any(pair => item.Equals(pair.Value));
        }

        public override void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw (new System.ArgumentNullException());
            }
            foreach (var pair in _pairs)
            {
                array[arrayIndex] = pair.Value;
                ++arrayIndex;
            }
        }

        public override IEnumerator<TValue> GetEnumerator()
        {
            return _pairs.Select(pair => pair.Value).GetEnumerator();
        }

        public override int Count => _pairs.Count;
    }
}
