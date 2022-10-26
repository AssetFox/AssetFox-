using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class InvestmentPlanRepositoryMocks
    {
        public static Mock<IInvestmentPlanRepository> NewMock(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IInvestmentPlanRepository>();
            unitOfWork?.Setup(u => u.InvestmentPlanRepo).Returns(mock.Object);
            return mock;
        }
    }
}
