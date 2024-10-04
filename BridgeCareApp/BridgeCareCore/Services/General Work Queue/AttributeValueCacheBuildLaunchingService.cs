using System;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class AttributeValueCacheBuildLaunchingService : BackgroundService
    {
        private SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;

        public AttributeValueCacheBuildLaunchingService(
            SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue
            )
        {
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
         
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workItem = new AggregatedResultCacheWorkItem();
            await _sequentialWorkQueue.Enqueue(workItem, out _);
        }
    }
}
