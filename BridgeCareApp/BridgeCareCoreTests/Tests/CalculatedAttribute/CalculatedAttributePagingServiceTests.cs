using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
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


        [Fact]
        public void GetSyncedScenarioDataset_RepoReturnsDto_ReturnsDto()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CalculatedAttributeRepositoryMocks.New(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var equationCriterionPairId = Guid.NewGuid();
            var equationId = Guid.NewGuid();
            var dto = CalculatedAttributeDtos.Age(id1, equationCriterionPairId, equationId);
            var cloneDto = CalculatedAttributeDtos.Age(id1, equationCriterionPairId, equationId);
            repo.Setup(r => r.GetScenarioCalculatedAttributes(simulationId)).Returns(new List<CalculatedAttributeDTO> { dto });
            var request = new CalculatedAttributePagingSyncModel
            {

            };

            var result = pagingService.GetSyncedScenarioDataSet(simulationId, request);

            var returnedDto = result.Single();
            ObjectAssertions.Equivalent(cloneDto, returnedDto);
        }
    }
}
