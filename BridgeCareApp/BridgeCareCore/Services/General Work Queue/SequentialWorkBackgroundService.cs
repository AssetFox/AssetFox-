using System;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class SequentialWorkBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SequentialWorkQueue<WorkQueueMetadata> _sequentialWorkQueue;

        public SequentialWorkBackgroundService(IServiceProvider serviceProvider, SequentialWorkQueue<WorkQueueMetadata> sequentialWorkQueue)
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
