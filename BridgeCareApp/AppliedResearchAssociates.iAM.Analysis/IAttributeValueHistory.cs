using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public interface IAttributeValueHistory<T> : IEnumerable<KeyValuePair<int, T>>
    {
        IEnumerable<int> Keys { get; }

        T MostRecentValue { get; set; }

        T this[int key] { get; set; }

        void Add(int key, T value);

        void Add(KeyValuePair<int, T> item);

        bool TryGetValue(int key, out T value);
    }
}
