using System;
using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class AttributeValueHistory<T> : IAttributeValueHistory<T>
    {
        public IEnumerable<int> Keys => History.Keys;

        public T MostRecentValue
        {
            get => _HasMostRecentValue ? _MostRecentValue : Attribute.DefaultValue;
            set
            {
                _MostRecentValue = value;
                _HasMostRecentValue = true;
            }
        }

        public T this[int key]
        {
            get => History[key];
            set => History[key] = value;
        }

        public void Add(int key, T value) => History.Add(key, value);

        public void Add(KeyValuePair<int, T> item) => Add(item.Key, item.Value);

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => History.GetEnumerator();

        public bool TryGetValue(int key, out T value) => History.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal AttributeValueHistory(Attribute<T> attribute) => Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        private readonly Attribute<T> Attribute;
        private readonly Dictionary<int, T> History = new();
        private bool _HasMostRecentValue;
        private T _MostRecentValue;
    }
}
