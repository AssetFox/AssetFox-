using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using HotChocolate.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.Services
{
    public class AggregatedResultCacheWorkItem : IWorkSpecification<WorkQueueMetadata>
    {
        public string UserId => "system";

        public string WorkId => WorkQueueWorkIdFactory.CreateId(Guid.Empty, WorkType.Aggregation);

        public string WorkDescription => "Rebuild aggregated result cache";

        public string WorkName => WorkDescription;

        public WorkQueueMetadata Metadata => new()
        {
            WorkType = WorkType.SimulationAnalysis,
            DomainType = DomainType.Network,
            DomainId = Guid.Empty,
        };

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken) {
            var memos = EventMemoModelLists.GetFreshInstance("BuildCache");
            memos.Mark("start");
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var attributeRepository = unitOfWork.AttributeRepo;
            var aggregatedResultRepository = unitOfWork.AggregatedResultRepo;
            var allAttributes = attributeRepository.GetAttributes();
            var allNames = allAttributes.Select(a => a.Name).ToList();
            memos.Mark("attributes");
            var allDtos = aggregatedResultRepository.GetAggregatedResultsForAttributeNames(allNames);
            memos.Mark("aggregated results");
            var cache = serviceProvider.GetRequiredService<AggregatedSelectValuesResultDtoCache>();
            foreach (var dto in allDtos)
            {
                cache.SaveToCache(dto);
            }
            memos.Mark("done");
            var text = memos.ToMultilineString();
            File.WriteAllText("BuildCache.txt", text);
        }
        public void OnCompletion(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var message = $"Attribute value cache rebuilt {DateTime.Now}";
            Debug.WriteLine(message);
            _hubService.SendRealTimeMessage("system", HubConstant.BroadcastTaskCompleted, message);
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            string cacheRebuildError = "Error building attribute value cache";
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{cacheRebuildError}::NetworkAggregateAccess - {errorMessage}");

        }
        public void OnUpdate(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWorkQueueUpdate, WorkId);

        }
    }
}
