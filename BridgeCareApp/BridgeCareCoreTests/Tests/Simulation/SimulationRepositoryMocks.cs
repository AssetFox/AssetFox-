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
    public static class SimulationRepositoryMocks
    {
        public static Mock<ISimulationRepository> DefaultMock(Mock<IUnitOfWork> unitOfWork = null)
        {
            var repository = new Mock<ISimulationRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.SimulationRepo).Returns(repository.Object);
            }
            return repository;
        }
    }
}
