﻿using System;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectControllerTests
    {
        private TestHelper _testHelper => TestHelper.Instance;
        private Mock<IUnitOfWork> _mockUOW;
        private Mock<ICommittedProjectService> _mockService;
        private Mock<ICommittedProjectRepository> _mockCommittedProjectRepo;
        private Guid _badScenario = Guid.Parse("0c66674c-8fcb-462b-8765-69d6815e0958");

        public CommittedProjectControllerTests()
        {
            _mockUOW = new Mock<IUnitOfWork>();
            // This is the DEFAULT current user (a user in the admin role)
            // It MUST be changed if testing for an unauthorized user.
            _mockUOW.Setup(_ => _.CurrentUser).Returns(AdminUser);

            var mockSimulationRepo = new Mock<ISimulationRepository>();
            mockSimulationRepo.Setup(_ => _.GetSimulation(It.Is<Guid>(_ => SimulationInTestData(_))))
                .Returns(TestDataForCommittedProjects.AuthorizedSimulationDTOs().First());
            mockSimulationRepo.Setup(_ => _.GetSimulation(It.Is<Guid>(_ => !SimulationInTestData(_))))
                .Throws<RowNotInTableException>();
            _mockUOW.Setup(_ => _.SimulationRepo).Returns(mockSimulationRepo.Object);

            _mockCommittedProjectRepo = new Mock<ICommittedProjectRepository>();
            _mockUOW.Setup(_ => _.CommittedProjectRepo).Returns(_mockCommittedProjectRepo.Object);

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(_ => _.UserExists(It.IsAny<string>())).Returns(true);
            _mockUOW.Setup(_ => _.UserRepo).Returns(mockUserRepo.Object);

            _mockService = new Mock<ICommittedProjectService>();
            _mockService.Setup(_ => _.ExportCommittedProjectsFile(It.IsAny<Guid>()))
                .Returns(TestDataForCommittedProjects.GoodFile());
        }

        [Fact]
        public async Task ExportWorksOnValidUser()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

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
            var accessor = HttpContextAccessorMocks.Default();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.ExportCommittedProjects(TestDataForCommittedProjects.Simulations.First().Id);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ImportWorksWithValidData()
        {
            // Arrange
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var hubService = HubServiceMocks.Default();
            mockContextAccessor.Setup(_ => _.HttpContext)
                .Returns(CreateLoadedContextForSimulation(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee")));
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                mockContextAccessor.Object);

            // Act
            var result = await controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(_ => _.ImportCommittedProjectFiles(It.IsAny<Guid>(), It.IsAny<ExcelPackage>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async Task ImportFailsIfUserUnauthorized()
        {
            // Arrange
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(_ => _.HttpContext)
                .Returns(CreateLoadedContextForSimulation(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee")));
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                mockContextAccessor.Object);

            // Act
            var result = await controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _mockService.Verify(_ => _.ImportCommittedProjectFiles(It.IsAny<Guid>(), It.IsAny<ExcelPackage>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task ImportFailsWithNoFile()
        {
            // Arrange
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(_ => _.HttpContext)
                .Returns(CreateContextWithNoFile(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee")));
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                mockContextAccessor.Object);

            // Act
            var result = await controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(_ => _.ImportCommittedProjectFiles(It.IsAny<Guid>(), It.IsAny<ExcelPackage>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task ImportFailsWithNoFormData()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(_ => _.ImportCommittedProjectFiles(It.IsAny<Guid>(), It.IsAny<ExcelPackage>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task ImportFailsWithNoMimeType()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _testHelper.UnitOfWork,
                hubService,
                accessor);

            // Act + Asset
            var result = await controller.ImportCommittedProjects();
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Committed Project error::Request MIME type is invalid.", ((BadRequestObjectResult)result).Value);
        }

        [Fact]
        public async Task ImportFailsOnNoSimulation()
        {
            // Arrange
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(_ => _.HttpContext)
                .Returns(CreateLoadedContextForSimulation(_badScenario));
            _mockService.Setup(_ => _.ImportCommittedProjectFiles(It.IsAny<Guid>(), It.IsAny<ExcelPackage>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<ArgumentException>();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                mockContextAccessor.Object);

            // Act
            var result = await controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSimulationCommittedProjects(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public async Task DeleteSimulationWorksWithValidSimulation()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.DeleteSimulationCommittedProjects(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));

            // Assert
            Assert.IsType<OkResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSimulationCommittedProjects(It.IsAny<Guid>()), Times.Once());
        }

        [Fact]
        public async Task DeleteSimulationFailsOnUnauthorizedUser()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.DeleteSimulationCommittedProjects(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSimulationCommittedProjects(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public async Task DeleteSimulationFailsOnBadSimulation()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.DeleteSimulationCommittedProjects(_badScenario);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSimulationCommittedProjects(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public async Task DeleteSpecificWorksWithValidProject()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            var deleteList = new List<Guid>()
            {
                Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"),
                Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00")
            };

            // Act
            var result = await controller.DeleteSpecificCommittedProjects(deleteList);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSpecificCommittedProjects(It.IsAny<List<Guid>>()), Times.Once());
        }

        [Fact]
        public async Task DeleteSpecificFailsOnUnauthorized()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            _mockCommittedProjectRepo.Setup(_ => _.GetSimulationId(It.IsAny<Guid>()))
                .Returns(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);
            var deleteList = new List<Guid>()
            {
                Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"),
                Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00")
            };

            // Act
            var result = await controller.DeleteSpecificCommittedProjects(deleteList);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.DeleteSpecificCommittedProjects(It.IsAny<List<Guid>>()), Times.Never());
        }

        [Fact]
        public async Task DeleteSpecificHandlesBadProject()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            var deleteList = new List<Guid>()
            {
                _badScenario
            };

            // Act
            var result = await controller.DeleteSpecificCommittedProjects(deleteList);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetSectionWorksOnValidUser()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockCommittedProjectRepo.Setup(_ => _.GetSectionCommittedProjectDTOs(It.IsAny<Guid>()))
                .Returns(TestDataForCommittedProjects.ValidCommittedProjects);
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.GetCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var contents = okResult.Value as List<SectionCommittedProjectDTO>;
            Assert.Equal(2, contents.Count);
        }

        [Fact]
        public async Task GetSectionFailsOnUnauthorized()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.GetCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetSectionHandlesBadScenario()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockCommittedProjectRepo.Setup(_ => _.GetSectionCommittedProjectDTOs(It.IsAny<Guid>()))
                .Throws<RowNotInTableException>();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.GetCommittedProjects(_badScenario);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpsertSectionWorksWithValidProjects()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                LibraryId = null,
                AddedRows = TestDataForCommittedProjects.ValidCommittedProjects,
                UpdateRows = new List<SectionCommittedProjectDTO>(),
                RowsForDeletion = new List<Guid>()
            };

            // Act
            var result = await controller.UpsertCommittedProjects(TestDataForCommittedProjects.ValidCommittedProjects[0].SimulationId, sync);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.UpsertCommittedProjects(It.IsAny<List<SectionCommittedProjectDTO>>()), Times.Once());
        }

        [Fact]
        public async Task UpsertFailsOnUnauthorized()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            _mockUOW.Setup(_ => _.CurrentUser).Returns(UnauthorizedUser);
            var hubService = HubServiceMocks.Default();

            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Dbe,
                _mockUOW.Object,
                hubService,
                accessor);

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                LibraryId = null,
                AddedRows = TestDataForCommittedProjects.ValidCommittedProjects,
                UpdateRows = new List<SectionCommittedProjectDTO>(),
                RowsForDeletion = new List<Guid>()
            };
            _mockService.Setup(_ => _.GetSyncedDataset(TestDataForCommittedProjects.ValidCommittedProjects[0].SimulationId, sync)).Returns(TestDataForCommittedProjects.ValidCommittedProjects);

            // Act
            var result = await controller.UpsertCommittedProjects(TestDataForCommittedProjects.ValidCommittedProjects[0].SimulationId, sync);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _mockCommittedProjectRepo.Verify(_ => _.UpsertCommittedProjects(It.IsAny<List<SectionCommittedProjectDTO>>()), Times.Never());
        }

        [Fact]
        public async Task UpsertHandlesBadScenario()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CommittedProjectController(
                _mockService.Object,
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            _mockCommittedProjectRepo.Setup(_ => _.UpsertCommittedProjects(It.IsAny<List<SectionCommittedProjectDTO>>()))
                .Throws<RowNotInTableException>();

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                LibraryId = null,
                AddedRows = TestDataForCommittedProjects.ValidCommittedProjects,
                UpdateRows = new List<SectionCommittedProjectDTO>(),
                RowsForDeletion = new List<Guid>()
            };


            // Act
            var result = await controller.UpsertCommittedProjects(TestDataForCommittedProjects.ValidCommittedProjects[0].SimulationId, sync);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
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

        private UserDTO UnauthorizedUser => new UserDTO
        {
            Username = "Nonadmin",
            HasInventoryAccess = true,
            Id = TestDataForCommittedProjects.UnauthorizedUser
        };

        private HttpContext CreateLoadedContextForSimulation(Guid simulationId)
        {
            var httpContext = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestCommittedProjects_Good.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestCommittedProjects_Good.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(simulationId.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection { formFile });
            return httpContext;
        }

        private HttpContext CreateContextWithNoFile(Guid simulationId)
        {
            var httpContext = new DefaultHttpContext();
            HttpContextSetup.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(simulationId.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData);
            return httpContext;
        }
        #endregion
    }
}
