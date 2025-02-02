﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates
{
    public sealed class SequenceEqualityComparer : EqualityComparer<IEnumerable>
    {
        new public static IEqualityComparer<IEnumerable> Default { get; } = new SequenceEqualityComparer(EqualityComparer<object>.Default);

        public static SequenceEqualityComparer Create(IEqualityComparer equalityComparer) => new SequenceEqualityComparer(equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer)));

        public static SequenceEqualityComparer<T> Create<T>(IEqualityComparer<T> equalityComparer) => new SequenceEqualityComparer<T>(equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer)));

        public override bool Equals(IEnumerable x, IEnumerable y) => Comparison.SequenceEquals(x, y, EqualityComparer);

        public override int GetHashCode(IEnumerable obj) => obj.CombineHashCodes(EqualityComparer);

        private readonly IEqualityComparer EqualityComparer;

        private SequenceEqualityComparer(IEqualityComparer equalityComparer) => EqualityComparer = equalityComparer;
    }

    public sealed class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
    {
        new public static IEqualityComparer<IEnumerable<T>> Default { get; } = new SequenceEqualityComparer<T>(EqualityComparer<T>.Default);

        public override bool Equals(IEnumerable<T> x, IEnumerable<T> y) => Comparison.SequenceEquals(x, y, EqualityComparer);

        public override int GetHashCode(IEnumerable<T> obj) => obj.CombineHashCodes(EqualityComparer);

        internal SequenceEqualityComparer(IEqualityComparer<T> equalityComparer) => EqualityComparer = equalityComparer;

        private readonly IEqualityComparer<T> EqualityComparer;
    }
}
