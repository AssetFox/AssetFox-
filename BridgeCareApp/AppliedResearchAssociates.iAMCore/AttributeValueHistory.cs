using System;
using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAMCore
{
    public sealed class AttributeValueHistory<T> : IDictionary<int, T>
    {
        public bool HasMostRecentValue
        {
            get => _HasMostRecentValue;
            set
            {
                _HasMostRecentValue = value;

                if (!_HasMostRecentValue)
                {
                    _MostRecentValue = default;
                }
            }
        }

        public T MostRecentValue
        {
            get => _HasMostRecentValue ? _MostRecentValue : Attribute.DefaultValue;
            set
            {
                _MostRecentValue = value;

                _HasMostRecentValue = true;
            }
        }

        public int Count => History.Count;

        public bool IsReadOnly => History.IsReadOnly;

        public ICollection<int> Keys => History.Keys;

        public ICollection<T> Values => History.Values;

        public T this[int key]
        {
            get => History[key];
            set => History[key] = value;
        }

        public void Add(int key, T value) => History.Add(key, value);

        public void Add(KeyValuePair<int, T> item) => History.Add(item);

        public void Clear() => History.Clear();

        public bool Contains(KeyValuePair<int, T> item) => History.Contains(item);

        public bool ContainsKey(int key) => History.ContainsKey(key);

        public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex) => History.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => History.GetEnumerator();

        public bool Remove(int key) => History.Remove(key);

        public bool Remove(KeyValuePair<int, T> item) => History.Remove(item);

        public bool TryGetValue(int key, out T value) => History.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)History).GetEnumerator();

        internal AttributeValueHistory(Attribute<T> attribute) => Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        private readonly Attribute<T> Attribute;

        private readonly IDictionary<int, T> History = new Dictionary<int, T>();

        private bool _HasMostRecentValue;

        private T _MostRecentValue;
    }
}
