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
    public static class CompleteSimulationRepositoryMocks
    {
        public static Mock<ICompleteSimulationRepository> DefaultMock(Mock<IUnitOfWork> unitOfWork = null)
        {
            var repository = new Mock<ICompleteSimulationRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.CompleteSimulationRepo).Returns(repository.Object);
            }
            return repository;
        }
    }
}
