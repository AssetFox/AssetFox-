using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class SequentialWorkQueueTests
    {
        [Fact]
        public void items_execute_in_the_order_they_were_added()
        {
            SequentialWorkQueue queue = new();
            List<int> taskEffects = new();

            queue.Add(new TestWorkItem(1, 0, taskEffects));
            queue.Add(new TestWorkItem(2, 1000, taskEffects));
            queue.Add(new TestWorkItem(3, 0, taskEffects));
            queue.Add(new TestWorkItem(4, 1000, taskEffects));
            queue.Add(new TestWorkItem(5, 0, taskEffects));

            queue.CompleteAdding();
            foreach (var workItem in queue.GetConsumingEnumerable())
            {
                workItem.DoWork(null);
            }

            Assert.Equal(Enumerable.Range(1, 5), taskEffects);
        }

        private record TestWorkItem(int Id, int MsDelay, List<int> WorkTarget) : IWorkItem
        {
            public void DoWork(IServiceProvider serviceProvider)
            {
                WorkTarget.Add(Id);
                Task.Delay(MsDelay).Wait();
            }
        }
    }
}
