using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class AggregatedSelectValuesResultDtoCacheService : BackgroundService
    {
        private readonly AggregatedSelectValuesResultDtoCache _cache;
        private readonly IServiceProvider _serviceProvider;

        public AggregatedSelectValuesResultDtoCacheService(
            AggregatedSelectValuesResultDtoCache cache,
            IServiceProvider serviceProvider)
        {
            _cache = cache;
            _serviceProvider = serviceProvider;
        }

        public void BuildCache()
        {
            var memos = EventMemoModelLists.GetFreshInstance("BuildCache");
            memos.Mark("start");
            var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            var attributeRepository = unitOfWork.AttributeRepo;
            var aggregatedResultRepository = unitOfWork.AggregatedResultRepo;
            var allAttributes = attributeRepository.GetAttributes();
            var allNames = allAttributes.Select(a => a.Name).ToList();
            memos.Mark("attributes");
            var allDtos = aggregatedResultRepository.GetAggregatedResultsForAttributeNames(allNames);
            memos.Mark("aggregated results");
            foreach (var dto in allDtos)
            {
                _cache.SaveToCache(dto);
            }
            memos.Mark("done");
            var text = memos.ToMultilineString();
            File.WriteAllText("BuildCache.txt", text);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => BuildCache());
        }
    }
}
