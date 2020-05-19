﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static partial class Static
    {
        public static IDisposable AsDisposable(this IEnumerable<IDisposable> source) => new AggregateDisposable(source);

        public static T? AsNullable<T>(this T value) where T : struct => value;

        public static IEnumerable<T?> AsNullables<T>(this IEnumerable<T> values) where T : struct => values.Select(AsNullable);

        public static IEnumerable<int> BoundRange(int start, int end, int stride = 1)
        {
            switch (Math.Sign(stride))
            {
            case 1:
                return towardPositiveInfinity();

            case -1:
                return towardNegativeInfinity();

            default:
                throw new ArgumentException("Stride must be non-zero.", nameof(stride));
            }

            IEnumerable<int> towardPositiveInfinity()
            {
                while (start <= end)
                {
                    yield return start;
                    start += stride;
                }
            }

            IEnumerable<int> towardNegativeInfinity()
            {
                while (start >= end)
                {
                    yield return start;
                    start += stride;
                }
            }
        }

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

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValue, out TKey key, out TValue value)
        {
            key = keyValue.Key;
            value = keyValue.Value;
        }

        public static IEnumerable<T> Distinct<T>(params T[] values) => values.Distinct();

        public static IEnumerable<T> Distinct<T>(IEqualityComparer<T> equalityComparer, params T[] values) => values.Distinct(equalityComparer);

        public static IEqualityComparer<KeyValuePair<TKey, TValue>> GetEqualityComparer<TKey, TValue>(this KeyValuePair<TKey, TValue> _, IEqualityComparer<TKey> keyEqualityComparer = null, IEqualityComparer<TValue> valueEqualityComparer = null) => new KeyValuePairEqualityComparer<TKey, TValue>(keyEqualityComparer, valueEqualityComparer);

        public static IEqualityComparer<T> GetEqualityComparerDefault<T>(this T _) => EqualityComparer<T>.Default;

        public static IEqualityComparer<TKey> GetKeyEqualityComparerDefault<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> _) => EqualityComparer<TKey>.Default;

        public static IEqualityComparer<KeyValuePair<TKey, TValue>> GetKeyValuePairEqualityComparer<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> _, IEqualityComparer<TKey> keyEqualityComparer = null, IEqualityComparer<TValue> valueEqualityComparer = null) => new KeyValuePairEqualityComparer<TKey, TValue>(keyEqualityComparer, valueEqualityComparer);

        public static IEqualityComparer<TValue> GetValueEqualityComparerDefault<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> _) => EqualityComparer<TValue>.Default;

        public static WeakReference<T> GetWeakReference<T>(this T reference) where T : class => new WeakReference<T>(reference);

        public static T Identity<T>(T value) => value;

        public static bool IsDefined<T>(this T enumValue) where T : Enum => Enum.IsDefined(typeof(T), enumValue);

        public static IEnumerable<IEnumerable<T>> SequenceCast<T>(this IEnumerable<IEnumerable> sequences) => sequences.Select(sequence => sequence.Cast<T>());

        public static int SequenceCompare(IEnumerable sequence1, IEnumerable sequence2, IComparer comparer = null) => _SequenceCompare(sequence1?.Cast<object>(), sequence2?.Cast<object>(), (comparer ?? Comparer<object>.Default).Compare);

        public static int SequenceCompare<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, IComparer<T> comparer = null) => _SequenceCompare(sequence1, sequence2, (comparer ?? Comparer<T>.Default).Compare);

        public static bool SequenceEquals(IEnumerable sequence1, IEnumerable sequence2, IEqualityComparer comparer = null) => _SequenceEquals(sequence1?.Cast<object>(), sequence2?.Cast<object>(), (comparer ?? EqualityComparer<object>.Default).Equals);

        public static bool SequenceEquals<T>(IEnumerable<T> sequence1, IEnumerable<T> sequence2, IEqualityComparer<T> comparer = null) => _SequenceEquals(sequence1, sequence2, (comparer ?? EqualityComparer<T>.Default).Equals);

        [Obsolete("Already present in netstandard2.1. Remove after upgrading.")]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> equalityComparer = null) => new HashSet<T>(source, equalityComparer);

        public static FinalActor<T> WithFinalAction<T>(this T value, Action<FinalActor<T>> finalAction) => new FinalActor<T>(value, finalAction ?? throw new ArgumentNullException(nameof(finalAction)));

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

        private sealed class AggregateDisposable : IDisposable
        {
            public AggregateDisposable(IEnumerable<IDisposable> disposables) => Disposables = disposables.ToList() ?? new List<IDisposable>();

            public void Dispose()
            {
                foreach (var disposable in Disposables)
                {
                    disposable?.Dispose();
                }
            }

            private readonly List<IDisposable> Disposables;
        }
    }
}
