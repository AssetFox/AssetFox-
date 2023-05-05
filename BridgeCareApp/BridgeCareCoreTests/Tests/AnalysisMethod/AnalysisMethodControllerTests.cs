using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.AnalysisMethod;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class AnalysisMethodControllerTests
    {
        private AnalysisMethodController CreateController(Mock<IUnitOfWork> mockUnitOfWork)
        {
            var claimHelper = ClaimHelperMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(mockUnitOfWork);
            var analysisDefaultDataServiceMock = AnalysisDefaultDataServiceMocks.DefaultMock();
            var controller = new AnalysisMethodController(
                EsecSecurityMocks.Admin,
                mockUnitOfWork.Object,
                HubServiceMocks.Default(),
                HttpContextAccessorMocks.Default(),
                analysisDefaultDataServiceMock.Object,
                claimHelper.Object);
            return controller;
        }

        [Fact]
        public async Task Get_RepositoryReturnsAnalysisMethodDto_ControllerReturnsOneToo()
        {
            var analysisMethodId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var dto = AnalysisMethodDtos.Default(analysisMethodId);
            var unitOfWork = UnitOfWorkMocks.New();
            var analysisMethodRepository = AnalysisMethodRepositoryMocks.DefaultMock(unitOfWork);
            analysisMethodRepository.Setup(a => a.GetAnalysisMethod(simulationId)).Returns(dto);
            var controller = CreateController(unitOfWork);

            var result = await controller.AnalysisMethod(simulationId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpsertAnalysisMethod_RepositoryDoesNotThrow_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repository = AnalysisMethodRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = AnalysisMethodDtos.Default(Guid.NewGuid());
            var simulationId = Guid.NewGuid();

            var result = await controller.UpsertAnalysisMethod(simulationId, dto);

            ActionResultAssertions.Ok(result);
            var invocation = repository.Invocations.Single();
            Assert.Equal(nameof(IAnalysisMethodRepository.UpsertAnalysisMethod), invocation.Method.Name);
            Assert.Equal(simulationId, invocation.Arguments[0]);
            Assert.Equal(dto, invocation.Arguments[1]);
        }
    }
}
