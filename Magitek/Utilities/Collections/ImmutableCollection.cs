using System.Collections.Generic;

namespace Magitek.Utilities.Collections
{
    public class ImmutableCollection<T> : ImmutableCollectionBase<T>
    {

        private readonly IList<T> _baseCollection;

        public ImmutableCollection(IEnumerable<T> source)
        {
            _baseCollection = new System.Collections.Generic.List<T>(source);
        }

        public ImmutableCollection()
        {
            _baseCollection = new System.Collections.Generic.List<T>();
        }

        public override int Count => _baseCollection.Count;

        public override bool Contains(T item)
        {
            return _baseCollection.Contains(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            _baseCollection.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return _baseCollection.GetEnumerator();
        }

    }
}
