using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using BridgeCareCore.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectControllerTests
    {
        private TestHelper _testHelper => TestHelper.Instance;
        private Mock<IUnitOfWork> _mockUOW;
        private Mock<ICommittedProjectService> _mockService;

        public CommittedProjectControllerTests()
        {
            _mockUOW = new Mock<IUnitOfWork>();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(AdminUser);

            var mockSimulationRepo = new Mock<ISimulationRepository>();
            mockSimulationRepo.Setup(_ => _.GetSimulation(It.Is<Guid>(_ => SimulationInTestData(_))))
                .Returns(TestDataForCommittedProjects.AuthorizedSimulationDTOs().First());
            mockSimulationRepo.Setup(_ => _.GetSimulation(It.Is<Guid>(_ => !SimulationInTestData(_))))
                .Throws<RowNotInTableException>();
            _mockUOW.Setup(_ => _.SimulationRepo).Returns(mockSimulationRepo.Object);

            _mockService = new Mock<ICommittedProjectService>();
            _mockService.Setup(_ => _.ExportCommittedProjectsFile(It.IsAny<Guid>()))
                .Returns(TestDataForCommittedProjects.GoodFile());
        }

        [Fact]
        public async Task ExportWorksOnValidUser()
        {
            // Arrange
            _testHelper.SetupDefaultHttpContext();
            var controller = new CommittedProjectController(
                _mockService.Object,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await controller.ExportCommittedProjects(TestDataForCommittedProjects.Simulations.First().Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var contents = okResult.Value as FileInfoDTO;
            Assert.Equal(TestDataForCommittedProjects.GoodFile().FileName, contents.FileName);
            Assert.True(contents.FileData.Length > 0);
        }

        [Fact]
        public async Task ExportFailsOnUnauthorized()
        {
            // Arrange
            _testHelper.SetupDefaultHttpContext();
            var controller = new CommittedProjectController(
                _mockService.Object,
                _testHelper.MockEsecSecurityNotAuthorized.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await controller.ExportCommittedProjects(TestDataForCommittedProjects.Simulations.First().Id);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        #region Helpers
        private bool SimulationInTestData(Guid simulationId) =>
            TestDataForCommittedProjects.Simulations.Any(_ => _.Id == simulationId);

        private UserDTO AdminUser => new UserDTO
        {
            Username = "Admin",
            HasInventoryAccess = true,
            Id = TestDataForCommittedProjects.AuthorizedUser
        };
        #endregion
    }
}
