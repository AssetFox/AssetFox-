using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public interface INetworkHistoryAccessor
    {
        void Clear();

        IReadOnlyList<KeyValuePair<int, T>> GetYears<T>(string asset, string attribute);

        void SetValue<T>(string asset, string attribute, int year, T value);

        bool TryGetValue<T>(string asset, string attribute, int year, out T value);
    }
}
