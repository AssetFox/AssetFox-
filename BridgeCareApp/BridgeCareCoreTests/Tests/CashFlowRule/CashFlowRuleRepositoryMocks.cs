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
    public class CashFlowRuleRepositoryMocks
    {
        public static Mock<ICashFlowRuleRepository> DefaultMock(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<ICashFlowRuleRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.CashFlowRuleRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
