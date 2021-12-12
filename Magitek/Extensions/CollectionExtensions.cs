using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Magitek.Extensions
{
    internal static class CollectionExtensions
    {
        /*
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
         
        {
            return new HashSet<T>(source);
        }
        */

        public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }
    }
}

