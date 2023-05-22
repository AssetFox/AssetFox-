using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis;

public interface IAttributeValueHistory
{
    string MostRecentValue { get; }
}

public interface IAttributeValueHistory<T> : IAttributeValueHistory, IEnumerable<KeyValuePair<int, T>>
{
    string IAttributeValueHistory.MostRecentValue => MostRecentValue.ToString();

    new T MostRecentValue { get; }

    int? MostRecentYear { get; }

    IEnumerable<int> Years { get; }

    T this[int year] { get; set; }

    void Add(int year, T value);

    void Add(KeyValuePair<int, T> yearValue);

    bool TryGetValue(int year, out T value);
}
