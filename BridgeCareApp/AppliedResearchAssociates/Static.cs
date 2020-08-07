using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static partial class Static
    {
        public static T? AsNullable<T>(this T value) where T : struct => value;

        public static IEnumerable<T?> AsNullables<T>(this IEnumerable<T> values) where T : struct => values.Select(AsNullable);

        public static void CopyFrom<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var (key, value) in source)
            {
                target[key] = value;
            }
        }

        public static IEnumerable<int> Count(int from = 0, int by = 1)
        {
            while (true)
            {
                yield return from;
                from += by;
            }
        }

        [Obsolete(NET_STANDARD_2_1_AVAILABILITY)]
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValue, out TKey key, out TValue value) => (key, value) = (keyValue.Key, keyValue.Value);

        public static T GetAdd<T>(this ICollection<T> collection, T value)
        {
            collection.Add(value);
            return value;
        }

        public static bool IsDefined<T>(this T enumValue) where T : Enum => Enum.IsDefined(typeof(T), enumValue);

        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            var item1 = list[index1];
            list[index1] = list[index2];
            list[index2] = item1;
        }

        [Obsolete(NET_STANDARD_2_1_AVAILABILITY)]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> equalityComparer = null) => new HashSet<T>(source, equalityComparer);

        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, IComparer<TKey> comparer = null) => new SortedDictionary<TKey, TValue>(source.ToDictionary(keySelector), comparer);

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer = null) => new SortedSet<T>(source, comparer);

        private const string NET_STANDARD_2_1_AVAILABILITY = "Already present in netstandard2.1. Remove after upgrading.";
    }
}
