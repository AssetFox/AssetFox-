using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public static class Extensions
    {
        public static void CopyFrom<TKey, TValue>(
            this IDictionary<TKey, TValue> target,
            IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var (key, value) in source)
            {
                target[key] = value;
            }
        }

        public static T GetAdd<T>(this ICollection<T> collection, T value)
        {
            collection.Add(value);
            return value;
        }

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer = null)
            => new SortedSet<T>(source, comparer);
    }
}
