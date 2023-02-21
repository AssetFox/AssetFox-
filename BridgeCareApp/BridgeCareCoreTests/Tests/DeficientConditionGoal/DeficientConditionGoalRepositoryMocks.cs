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
    public static class DeficientConditionGoalRepositoryMocks
    {
        public static Mock<IDeficientConditionGoalRepository> DefaultMock(Mock<IUnitOfWork> unitOfWorkMock = null)
        {
            var repo = new Mock<IDeficientConditionGoalRepository>();
            if (unitOfWorkMock != null)
            {
                unitOfWorkMock.Setup(u => u.DeficientConditionGoalRepo).Returns(repo.Object);
            }
            return repo;
        }
    }
}
