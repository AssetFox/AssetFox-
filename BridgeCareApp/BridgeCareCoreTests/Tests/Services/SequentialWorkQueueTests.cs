using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class SequentialWorkQueueTests
    {
        [Fact]
        public void items_execute_in_the_order_they_were_added()
        {
            SequentialWorkQueue queue = new();
            List<int> taskEffects = new();

            queue.Enqueue(new TestWorkItem(1, 0, taskEffects), out _).Wait();
            queue.Enqueue(new TestWorkItem(2, 1000, taskEffects), out _).Wait();
            queue.Enqueue(new TestWorkItem(3, 0, taskEffects), out _).Wait();
            queue.Enqueue(new TestWorkItem(4, 1000, taskEffects), out _).Wait();
            queue.Enqueue(new TestWorkItem(5, 0, taskEffects), out _).Wait();

            CancellationTokenSource cts = new();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var workStarter = queue.Dequeue(cts.Token).Result;
                    workStarter.StartWork(null);
                }
            }
            catch (AggregateException e) when (e.InnerException is OperationCanceledException)
            {
            }

            Assert.Equal(Enumerable.Range(1, 5), taskEffects);
        }

        private record TestWorkItem(int Id, int MsDelay, List<int> WorkTarget) : IWorkSpecification
        {
            public string WorkId { get; } = Id.ToString();

            public string UserId => "";

            public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatus, CancellationToken cancellationToken)
            {
                WorkTarget.Add(Id);
                Task.Delay(MsDelay).Wait();
            }
        }
    }
}
