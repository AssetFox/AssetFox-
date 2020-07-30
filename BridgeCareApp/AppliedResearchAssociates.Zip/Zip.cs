using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static partial class Zip
    {
        public static IEnumerable<T[]> Long<T>(IEnumerable<IEnumerable<T>> sequences, T defaultValue = default) => new LongZipper<T>(defaultValue).GetEnumerable(sequences);

        public static IEnumerable<object[]> Long(IEnumerable<IEnumerable> sequences, object defaultValue = default) => Long(sequences.SequenceCast<object>(), defaultValue);

        public static IEnumerable<T[]> Short<T>(IEnumerable<IEnumerable<T>> sequences) => ShortZipper<T>.Instance.GetEnumerable(sequences);

        public static IEnumerable<object[]> Short(IEnumerable<IEnumerable> sequences) => Short(sequences.SequenceCast<object>());

        public static IEnumerable<T[]> Strict<T>(IEnumerable<IEnumerable<T>> sequences) => StrictZipper<T>.Instance.GetEnumerable(sequences);

        public static IEnumerable<object[]> Strict(IEnumerable<IEnumerable> sequences) => Strict(sequences.SequenceCast<object>());

        private static IEnumerable<T> Distinct<T>(params T[] values) => values.Distinct();

        private static IEnumerable<IEnumerable<T>> SequenceCast<T>(this IEnumerable<IEnumerable> sequences) => sequences.Select(sequence => sequence.Cast<T>());

        private sealed class AggregateDisposable : IDisposable
        {
            private readonly List<IDisposable> Disposables;

            public AggregateDisposable(IEnumerable<IDisposable> disposables) => Disposables = disposables?.ToList() ?? new List<IDisposable>();

            public void Dispose()
            {
                foreach (var disposable in Disposables)
                {
                    disposable?.Dispose();
                }
            }
        }

        private abstract class AggregateEnumerator<T>
        {
            public IEnumerable<T[]> GetEnumerable(IEnumerable<IEnumerable<T>> sequences)
            {
                if (sequences == null)
                {
                    throw new ArgumentNullException(nameof(sequences));
                }

                return _();

                IEnumerable<T[]> _()
                {
                    var enumerators = sequences.Select(sequence => sequence.GetEnumerator()).ToList();
                    using (new AggregateDisposable(enumerators))
                    {
                        while (MoveNext(enumerators))
                        {
                            yield return Current(enumerators);
                        }
                    }
                }
            }

            protected virtual T[] Current(IEnumerable<IEnumerator<T>> enumerators) => enumerators.Select(enumerator => enumerator.Current).ToArray();

            protected abstract bool MoveNext(IEnumerable<IEnumerator<T>> enumerators);
        }

        private sealed class LongZipper<T> : AggregateEnumerator<T>
        {
            private readonly T DefaultValue;

            private readonly List<bool> HasCurrent = new List<bool>();

            public LongZipper(T defaultValue) => DefaultValue = defaultValue;

            protected override T[] Current(IEnumerable<IEnumerator<T>> enumerators)
            {
                return _().ToArray();

                IEnumerable<T> _()
                {
                    foreach (var (enumerator, hasCurrent) in Strict(enumerators, HasCurrent))
                    {
                        yield return hasCurrent ? enumerator.Current : DefaultValue;
                    }
                }
            }

            protected override bool MoveNext(IEnumerable<IEnumerator<T>> enumerators)
            {
                HasCurrent.Clear();
                HasCurrent.AddRange(enumerators.Select(enumerator => enumerator.MoveNext()));

                return HasCurrent.Any(_ => _);
            }
        }

        private sealed class ShortZipper<T> : AggregateEnumerator<T>
        {
            public static readonly ShortZipper<T> Instance = new ShortZipper<T>();

            private ShortZipper()
            {
            }

            protected override bool MoveNext(IEnumerable<IEnumerator<T>> enumerators)
            {
                var any = false;
                var moveNext = true;

                foreach (var enumerator in enumerators)
                {
                    any = true;
                    moveNext &= enumerator.MoveNext();
                }

                return any && moveNext;
            }
        }

        private sealed class StrictZipper<T> : AggregateEnumerator<T>
        {
            public static readonly StrictZipper<T> Instance = new StrictZipper<T>();

            private StrictZipper()
            {
            }

            protected override bool MoveNext(IEnumerable<IEnumerator<T>> enumerators) => enumerators.Select(enumerator => enumerator.MoveNext()).Distinct().SingleOrDefault();
        }
    }
}
