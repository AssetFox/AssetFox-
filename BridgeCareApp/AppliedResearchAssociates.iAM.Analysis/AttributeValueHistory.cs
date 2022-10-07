using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class AttributeValueHistory<T> : IEnumerable<KeyValuePair<int, T>>
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

        public IEnumerable<int> Keys => this.Select(kv => kv.Key);

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
            get => TryGetValue(key, out var value) ? value : throw new ArgumentException("Key not found.", nameof(key));
            set => Add(key, value);
        }

        public void Add(int key, T value) => Accessor.SetValue(Asset.AssetName, Attribute.Name, key, value);

        public void Add(KeyValuePair<int, T> item) => Add(item.Key, item.Value);

        public bool TryGetValue(int key, out T value) => Accessor.TryGetValue(Asset.AssetName, Attribute.Name, key, out value);

        internal AttributeValueHistory(AnalysisMaintainableAsset asset, Attribute<T> attribute)
        {
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

            Accessor = Asset.Network.History.GetAccessor();
        }

        private readonly INetworkHistoryAccessor Accessor;
        private readonly AnalysisMaintainableAsset Asset;
        private readonly Attribute<T> Attribute;
        private bool _HasMostRecentValue;
        private T _MostRecentValue;

        #region IEnumerable<KeyValuePair<int, T>>

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => Accessor.GetYears<T>(Asset.AssetName, Attribute.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable<KeyValuePair<int, T>>
    }
}
