using System;
using System.Threading;
using System.Threading.Tasks;
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
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            // Go asynchronous ASAP to avoid blocking the caller (and the whole app).
            await Task.Yield();

            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            foreach (var workItem in _sequentialWorkQueue)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                workItem.DoWork(_serviceProvider);

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
