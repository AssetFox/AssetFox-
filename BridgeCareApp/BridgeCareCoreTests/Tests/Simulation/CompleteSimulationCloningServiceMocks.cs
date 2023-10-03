using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Services;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class CompleteSimulationCloningServiceMocks
    {

        public static Mock<ICompleteSimulationCloningService> New()
        {
            var mock = new Mock<ICompleteSimulationCloningService>();
            return mock;
        }
    }
}
