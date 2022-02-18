using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class SequentialTaskQueueTests
    {
        [Fact]
        public void tasks_execute_in_order_they_were_added()
        {
            SequentialTaskQueue queue = new();
            List<int> taskEffects = new();

            void addTestTask(int id, int ms) => queue.Add(new(() =>
            {
                taskEffects.Add(id);
                Task.Delay(ms).Wait();
            }));

            addTestTask(1, 0);
            addTestTask(2, 1000);
            addTestTask(3, 0);
            addTestTask(4, 1000);
            addTestTask(5, 0);

            queue.CompleteAdding();
            queue.Consumer.Wait();

            Assert.Equal(Enumerable.Range(1, 5), taskEffects);
        }
    }
}
