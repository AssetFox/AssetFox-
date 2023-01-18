using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CriterionLibraryControllerTests
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
        public async Task CriterionLibraries_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.CriterionLibraries();

            // Assert
            ActionResultAssertions.OkObject(result);
            criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.CriterionLibraries));
        }

        [Fact]
        public async Task UpsertCriterionLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var repo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = CriterionLibraryTestSetup.TestCriterionLibrary();

            // Act
            var result = await controller
                .UpsertCriterionLibrary(dto);

            // Assert
            ActionResultAssertions.OkObject(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.UpsertCriterionLibrary));
            Assert.Equal(dto, invocation.Arguments[0]);
        }

        [Fact]
        public async Task DeleteCriterionLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            UserRepositoryMocks.EveryoneExists(unitOfWork);
            var repo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            // Act
            var result = await controller.DeleteCriterionLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.DeleteCriterionLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
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
    }
}
