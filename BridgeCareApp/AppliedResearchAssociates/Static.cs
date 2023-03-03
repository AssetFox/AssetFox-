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

        public static void DecrementIndexOf<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index > 0)
            {
                list.Swap(index - 1, index);
            }
        }

        public static T GetAdd<T>(this ICollection<T> collection, T value)
        {
            collection.Add(value);
            return value;
        }

        public static void IncrementIndexOf<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index >= 0 && index < list.Count - 1)
            {
                list.Swap(index, index + 1);
            }
        }

        public static bool IsDefined<T>(this T enumValue) where T : Enum => Enum.IsDefined(typeof(T), enumValue);

        public static bool IsSorted<T>(this IList<T> list, IComparer<T> comparer = null)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count < 2)
            {
                return true;
            }

            comparer = comparer ?? Comparer<T>.Default;

            var e0 = list[0];
            var e1 = list[1];

            var order = Math.Sign(comparer.Compare(e0, e1));

            var i = 2;
            for (; i < list.Count && order == 0; ++i)
            {
                e0 = e1;
                e1 = list[i];

                order = Math.Sign(comparer.Compare(e0, e1));
            }

            for (; i < list.Count; ++i)
            {
                e0 = e1;
                e1 = list[i];

                if (order == -Math.Sign(comparer.Compare(e0, e1)))
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<T> Once<T>(this T value) => Enumerable.Repeat(value, 1);

        public static IEnumerable<int> RangeFromBounds(int start, int end, int stride = 1)
        {
            switch (Math.Sign(stride))
            {
            case 1:
                return TowardPositiveInfinity(start, end, stride);

            case -1:
                return TowardNegativeInfinity(start, end, stride);

            default:
                throw new ArgumentException("Stride must be non-zero.", nameof(stride));
            };
        }

        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            var item1 = list[index1];
            list[index1] = list[index2];
            list[index2] = item1;
        }

        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, IComparer<TKey> comparer = null) => new SortedDictionary<TKey, TValue>(source.ToDictionary(keySelector), comparer);

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer = null) => new SortedSet<T>(source, comparer);

        private static IEnumerable<int> TowardNegativeInfinity(int start, int end, int stride)
        {
            while (start >= end)
            {
                yield return start;
                start += stride;
            }
        }

        private static IEnumerable<int> TowardPositiveInfinity(int start, int end, int stride)
        {
            while (start <= end)
            {
                yield return start;
                start += stride;
            }
        }
    }
}
