using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CalculatedAttributePagingServiceTests
    {
        private CalculatedAttributePagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CalculatedAttributePagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO>());
            var request = new CalculatedAttributePagingSyncModel
            {

            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            Assert.Empty(result);
        }
    }
}
