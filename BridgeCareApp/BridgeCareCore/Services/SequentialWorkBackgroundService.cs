using System;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore.Services
{
    public class SequentialWorkBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SequentialWorkQueue _sequentialWorkQueue;

        public SequentialWorkBackgroundService(IServiceProvider serviceProvider, SequentialWorkQueue sequentialWorkQueue)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _sequentialWorkQueue = sequentialWorkQueue ?? throw new ArgumentNullException(nameof(sequentialWorkQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _sequentialWorkQueue.Dequeue(stoppingToken);
                workItem?.DoWork(_serviceProvider, stoppingToken);
            }
        }
    }
}
