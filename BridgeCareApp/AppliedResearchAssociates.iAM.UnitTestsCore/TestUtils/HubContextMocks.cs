using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class HubContextMocks
    {
        public static Mock<IHubContext<BridgeCareHub>> DefaultMock()
        {
            var mock = new Mock<IHubContext<BridgeCareHub>>();
            return mock;
        }

        public static void GetStuff(this Mock<IHubContext<BridgeCareHub>> mock)
        {
            var invocations = mock.Invocations.ToList();
        }
    }
}
