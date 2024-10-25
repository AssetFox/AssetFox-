using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public interface IAggregatedSelectValuesResultDtoCache
    {
        void ClearInvalid();
        void SaveToCache(AggregatedSelectValuesResultDTO dto);
        AggregatedSelectValuesResultDTO TryGetCachedValue(string attributeName);
    }
}
