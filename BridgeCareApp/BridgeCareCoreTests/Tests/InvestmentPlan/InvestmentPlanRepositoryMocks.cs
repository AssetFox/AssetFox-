using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class InvestmentPlanRepositoryMocks
    {
        public static Mock<IInvestmentPlanRepository> NewMock()
        {
            var mock = new Mock<IInvestmentPlanRepository>();
            return mock;
        }
    }
}
