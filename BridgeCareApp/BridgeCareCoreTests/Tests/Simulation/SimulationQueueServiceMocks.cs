using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class SimulationQueueServiceMocks
    {
        public static Mock<ISimulationQueueService> New()
        {
            var mock = new Mock<ISimulationQueueService>();
            return mock;
        }
    }
}
