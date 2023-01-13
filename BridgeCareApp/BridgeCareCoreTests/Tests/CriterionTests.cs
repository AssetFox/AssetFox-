using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CriterionTests
    {

        private CriterionLibraryController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CashFlowPagingService(unitOfWork.Object);
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var controller = new CriterionLibraryController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object
                );
            return controller;
        }
        private CriterionLibraryController SetupController()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new CriterionLibraryController(
                EsecSecurityMocks.Admin,
                unitOfWork,
                hubService,
                accessor);
            return controller;
        }

        private CriterionLibraryDTO Setup()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            return criterionLibrary;
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = SetupController();
            // Act
            var result = await controller.CriterionLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var controller = SetupController();
            // Act
            var result = await controller
                .UpsertCriterionLibrary(CriterionLibraryTestSetup.TestCriterionLibrary());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var controller = SetupController();
            // Act
            var result = await controller.DeleteCriterionLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllCriterionLibraries()
        {
            var controller = SetupController();
            // Arrange
            var criterionLibrary = Setup();

            // Act
            var result = await controller.CriterionLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CriterionLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CriterionLibraryDTO>));
            Assert.Contains(dtos, cl => cl.Id == criterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldDeleteCriterionLibrary()
        {
            // Arrange
            var controller = SetupController();
            var criterionLibrary = Setup();

            // Act
            var result = await controller.DeleteCriterionLibrary(criterionLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibrary.Any(_ =>
                    _.Id == criterionLibrary.Id));
        }
    }
}
