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
using Microsoft.Graph.Models;
using System.Collections.Generic;

namespace BridgeCareCore.Services
{
    public class AggregatedResultCacheWorkItem : IWorkSpecification<WorkQueueMetadata>
    {
        public string UserId => "system";

        public readonly Guid Id;

        public string WorkId => WorkQueueWorkIdFactory.CreateId(Id, WorkType.Aggregation);

        public string WorkDescription => "Rebuild aggregated result cache";

        public string WorkName => WorkDescription;

        public AggregatedResultCacheWorkItem()
        {
            Id = Guid.NewGuid();
        }

        public WorkQueueMetadata Metadata => new()
        {
            WorkType = WorkType.SimulationAnalysis,
            DomainType = DomainType.Network,
            DomainId = Guid.Empty,
        };

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken) {
            var memos = EventMemoModelLists.GetFreshInstance("BuildCache");
            memos.Mark("start");
            using var scope = serviceProvider.CreateScope();
            var scopeProvider = scope.ServiceProvider;
            var unitOfWork = scopeProvider.GetRequiredService<IUnitOfWork>();
            var attributeRepository = unitOfWork.AttributeRepo;
            var aggregatedResultRepository = unitOfWork.AggregatedResultRepo;
            var allAttributes = attributeRepository.GetAttributes();
            var allNames = allAttributes.Select(a => a.Name).ToList();
            var cache = serviceProvider.GetRequiredService<AggregatedSelectValuesResultDtoCache>();
            var tooBig = cache.AttributesTooBigToCache;
            var attributesToCache = allNames.Except(tooBig).ToList();
            var batches = new List<List<string>>();
            while(attributesToCache.Any())
            {
                var batch = attributesToCache.Take(10).ToList();
                batches.Add(batch);
                attributesToCache = attributesToCache.Skip(10).ToList();
            }
            memos.Mark("attributes");
            foreach (var batch in batches)
            {
                var allDtos = aggregatedResultRepository.GetAggregatedResultsForAttributeNames(batch);
                foreach (var dto in allDtos)
                {
                    cache.SaveToCache(dto);
                }
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
