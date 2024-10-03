using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Services
{
    public class AggregatedSelectValuesResultDtoCacheService
    {
        private readonly AggregatedSelectValuesResultDtoCache _cache;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IAggregatedResultRepository _aggregatedResultRepository;

        public AggregatedSelectValuesResultDtoCacheService(
            AggregatedSelectValuesResultDtoCache cache,
            IAttributeRepository attributeRepository,
            IAggregatedResultRepository aggregatedResultRepository)
        {
            _cache = cache;
            _attributeRepository = attributeRepository;
            _aggregatedResultRepository = aggregatedResultRepository;
        }

        public void BuildCache()
        {
            var allAttributes = _attributeRepository.GetAttributes();
            var allNames = allAttributes.Select(a => a.Name).ToList();
            var allDtos = _aggregatedResultRepository.GetAggregatedResultsForAttributeNames(allNames);
            foreach (var dto in allDtos)
            {
                _cache.SaveToCache(dto);
            }
        }
    }
}
