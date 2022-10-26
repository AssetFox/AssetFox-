using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public interface IAttributeValueHistory<T> : IEnumerable<KeyValuePair<int, T>>
    {
        T MostRecentValue { get; }

        int? MostRecentYear { get; }

        IEnumerable<int> Years { get; }

        T this[int year] { get; set; }

        void Add(int year, T value);

        void Add(KeyValuePair<int, T> yearValue);

        bool TryGetValue(int year, out T value);
    }
}
