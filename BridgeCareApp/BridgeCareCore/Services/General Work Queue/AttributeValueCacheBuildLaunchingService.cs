using System;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class AttributeValueCacheBuildLaunchingService : BackgroundService
    {
        private SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;
        private Timer _cacheRebuildTimer;

        public AttributeValueCacheBuildLaunchingService(
            SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue
            )
        {
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LaunchQueueBuild(null);
            CreateTimer();
            await Task.CompletedTask;
        }

        private void LaunchQueueBuild(object state)
        {
            var workItem = new AggregatedResultCacheWorkItem();
            _sequentialWorkQueue.Enqueue(workItem, out _);
        }

        private void CreateTimer()
        {
            // cache rebuild timer should fire at 7am and 7pm every day
            var now = DateTime.Now;
            var sevenAm = new DateTime(now.Year, now.Month, now.Day).AddHours(7);
            var difference = sevenAm - now;
            while (difference < TimeSpan.Zero)
            {
                difference = difference + TimeSpan.FromHours(12);
            }
            _cacheRebuildTimer = new Timer(LaunchQueueBuild, null, difference, TimeSpan.FromHours(12));
        }
    }
}
