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
    public static class TargetConditionGoalRepositoryMocks
    {
        public static Mock<ITargetConditionGoalRepository> New(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var mock = new Mock<ITargetConditionGoalRepository>();
            if (mockUnitOfWork != null)
            {
                mockUnitOfWork.Setup(m => m.TargetConditionGoalRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
