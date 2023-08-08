using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
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

        public static void SetupGetSimulation(this Mock<ISimulationRepository> mock, SimulationDTO simulation)
        {
            mock.Setup(s => s.GetSimulation(simulation.Id)).Returns(simulation);
        }
    }
}
