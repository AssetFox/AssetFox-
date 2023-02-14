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
        private static Dictionary<string, string> _createErrorList()
        {
            var dummyHubContext = new Mock<IHubContext<BridgeCareHub>>();
            var dummy = new HubService(dummyHubContext.Object);
            var castDummy = dummy as IHubService;
            var dictionary = castDummy.errorList;
            return dictionary;
        }

        public static Mock<IHubService> DefaultMock()
        {
            var mock = new Mock<IHubService>();
            var errorList = _createErrorList();
            mock.Setup(m => m.errorList).Returns(errorList);
            return mock;
        }

        public static IHubService Default()
        {
            var mock = DefaultMock();
            return mock.Object;
        }

        public static List<string> ThreeArgumentUserMessages(this Mock<IHubService> mock)
        {
            var invocations = mock.Invocations.ToList();
            var realTimeMessageInvocations = invocations.Where(i => i.Method.Name == nameof(IHubService.SendRealTimeMessage)).ToList();
            var threeArgumentInvocations = realTimeMessageInvocations.Where(i => i.Arguments.Count == 3).ToList();
            var threeArgumentInvocationFinalArguments = threeArgumentInvocations.Select(i => i.Arguments[2].ToString()).ToList();
            return threeArgumentInvocationFinalArguments;
        }

        public static string SingleThreeArgumentUserMessage(this Mock<IHubService> mock)
        {
            var messages = mock.ThreeArgumentUserMessages();
            var message = messages.Single();
            return message;
        }
    }
}
