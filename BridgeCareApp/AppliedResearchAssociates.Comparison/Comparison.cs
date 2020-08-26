using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static class Comparison
    {
        public static int CombineHashCodes(this IEnumerable values, IEqualityComparer equalityComparer = null) => values.Cast<object>().CombineHashCodes(equalityComparer);

        public static int CombineHashCodes<T>(this IEnumerable<T> values, IEqualityComparer<T> equalityComparer = null)
        {
            var result = new HashCode();
            foreach (var value in values)
            {
                result.Add(value, equalityComparer);
            }

            return result.ToHashCode();
        }

        public static int SequenceCompare(IEnumerable sequence1, IEnumerable sequence2, IComparer comparer = null) => _SequenceCompare(sequence1?.Cast<object>(), sequence2?.Cast<object>(), (comparer ?? Comparer<object>.Default).Compare);

        public static int SequenceCompare<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, IComparer<T> comparer = null) => _SequenceCompare(sequence1, sequence2, (comparer ?? Comparer<T>.Default).Compare);

        public static bool SequenceEquals(IEnumerable sequence1, IEnumerable sequence2, IEqualityComparer comparer = null) => _SequenceEquals(sequence1?.Cast<object>(), sequence2?.Cast<object>(), (comparer ?? EqualityComparer<object>.Default).Equals);

        public static bool SequenceEquals<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, IEqualityComparer<T> comparer = null) => _SequenceEquals(sequence1, sequence2, (comparer ?? EqualityComparer<T>.Default).Equals);

        private static int _SequenceCompare<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, Func<T, T, int> compare)
        {
            if (ReferenceEquals(sequence1, sequence2))
            {
                return 0;
            }

            if (sequence1 is null)
            {
                return -1;
            }

            if (sequence2 is null)
            {
                return 1;
            }

            var enumerator1 = sequence1.GetEnumerator();
            var enumerator2 = sequence2.GetEnumerator();

            bool sequence1HasValue, sequence2HasValue;
            while ((sequence1HasValue = enumerator1.MoveNext()) & (sequence2HasValue = enumerator2.MoveNext()))
            {
                var comparison = compare(enumerator1.Current, enumerator2.Current);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            if (!sequence1HasValue && sequence2HasValue)
            {
                return -1;
            }

            if (sequence1HasValue && !sequence2HasValue)
            {
                return 1;
            }

            return 0;
        }

        private static bool _SequenceEquals<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, Func<T, T, bool> equals)
        {
            if (ReferenceEquals(sequence1, sequence2))
            {
                return true;
            }

            if (sequence1 is null || sequence2 is null)
            {
                return false;
            }

            var enumerator1 = sequence1.GetEnumerator();
            var enumerator2 = sequence2.GetEnumerator();

            bool sequence1HasValue, sequence2HasValue;
            while ((sequence1HasValue = enumerator1.MoveNext()) & (sequence2HasValue = enumerator2.MoveNext()))
            {
                var comparison = equals(enumerator1.Current, enumerator2.Current);
                if (!comparison)
                {
                    return false;
                }
            }

            return sequence1HasValue == sequence2HasValue;
        }
    }
}
