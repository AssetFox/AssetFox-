using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class HubServiceMocks
    {
        public static Mock<IHubContext<BridgeCareHub>> MockHubContext()
        {
            var mock = new Mock<IHubContext<BridgeCareHub>>();
            return mock;
        }

        public static Mock<HubService> DefaultMock()
        {
            var context = MockHubContext();
            var mock = new Mock<HubService>(context.Object);
            return mock;
        }

        public static HubService Default()
        {
            var mock = DefaultMock();
            return mock.Object;
        }
    }
}
