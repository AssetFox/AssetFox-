using System;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class FastSequentialWorkBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FastSequentialworkQueue<WorkQueueMetadata> _sequentialWorkQueue;

        public FastSequentialWorkBackgroundService(IServiceProvider serviceProvider, FastSequentialworkQueue<WorkQueueMetadata> sequentialWorkQueue)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workStarter = await _sequentialWorkQueue.Dequeue(stoppingToken);
                workStarter?.StartWork(_serviceProvider);
            }
        }
    }
}
