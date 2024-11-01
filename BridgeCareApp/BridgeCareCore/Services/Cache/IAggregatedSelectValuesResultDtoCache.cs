using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public interface IAggregatedSelectValuesResultDtoCache
    {
        List<string> AttributesTooBigToCache { get; }

        void ClearInvalid();
        void SaveToCache(AggregatedSelectValuesResultDTO dto);
        AggregatedSelectValuesResultDTO TryGetCachedValue(string attributeName);
    }
}
