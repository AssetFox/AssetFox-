using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CashFlowControllerTests
    {
        private CashFlowController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CashFlowPagingService(unitOfWork.Object);
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var accessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var controller = new CashFlowController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                accessor.Object,
                claimHelper.Object,
                service
                );
            return controller;
        }

        [Fact]
        public async Task GetCashFlowRuleLibraries_CallsGetNoChildrenOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.GetCashFlowRuleLibraries();

            // Assert
            ActionResultAssertions.OkObject(result);
            repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.GetCashFlowRuleLibrariesNoChildren));
        }

        [Fact]
        public async Task ShouldGetLibraryTargetConditionGoalPageData()
        {
            // Paging service test
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var libraryId = Guid.NewGuid();
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).Returns(rules);
            var controller = CreateController(unitOfWork);

            // Act
            var request = new PagingRequestModel<CashFlowRuleDTO>();
            var result = await controller.GetLibraryCashFlowRulePage(libraryId, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            var page = (PagingPageModel<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<CashFlowRuleDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(rule, dto);
        }

        [Fact]
        public async Task GetScenarioCashFlowRulePage_GetsFromRepo()
        {
            // paging service test
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            repo.Setup(r => r.GetScenarioCashFlowRules(simulationId)).Returns(rules);

            // Act
            var request = new PagingRequestModel<CashFlowRuleDTO>();
            var result = await controller.GetScenarioCashFlowRulePage(simulationId, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            var page = (PagingPageModel<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<CashFlowRuleDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(rule, dto);
        }

        [Fact]
        public async Task GetScenarioCashFlowRules_GetsFromRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();

            // Act
            var result = await controller.GetScenarioCashFlowRules(simulationId);

            // Assert
            ActionResultAssertions.OkObject(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.GetScenarioCashFlowRules));
            Assert.Equal(simulationId, invocation.Arguments[0]);
        }

        [Fact]
        public async Task UpsertCashFlowRuleLibrary_CallsUpsertOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = new CashFlowRuleLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                CashFlowRules = new List<CashFlowRuleDTO>()
            };
            var dtoClone = new CashFlowRuleLibraryDTO
            {
                Id = dto.Id,
                Name = "",
                CashFlowRules = new List<CashFlowRuleDTO>(),
            };
            var libraryRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>()
            {
                IsNewLibrary = true,
                Library = dto,
            };
            var libraryDoesNotExist = LibraryAccessModels.LibraryDoesNotExist();
            repo.SetupGetLibraryAccess(dto.Id, libraryDoesNotExist);

            // Act
            var result = await controller.UpsertCashFlowRuleLibrary(libraryRequest);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryInvocation = repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.UpsertCashFlowRuleLibraryAndRules));
            var inputDto = libraryInvocation.Arguments[0] as CashFlowRuleLibraryDTO;
            ObjectAssertions.Equivalent(dtoClone, inputDto);
        }

        [Fact]
        public async Task UpsertOrDeleteScenarioCashFlowRules_CallsUpsertOrDeleteOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dtos = new List<CashFlowRuleDTO>();
            var sync = new PagingSyncModel<CashFlowRuleDTO>();
            var simulationId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCashFlowRules(simulationId)).Returns(new List<CashFlowRuleDTO>());
            // Act
            var result = await controller.UpsertScenarioCashFlowRules(simulationId, sync);

            // Assert
            ActionResultAssertions.Ok(result);
            var invocation = repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.UpsertOrDeleteScenarioCashFlowRules));
            Assert.Equal(simulationId, invocation.Arguments[1]);
        }

        [Fact]
        public async Task DeleteCashFlowRuleLibrary_CallsDeleteOnRepo()
        {

            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            // Act
            var result = await controller.DeleteCashFlowRuleLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);
            var repoInvocation = repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.DeleteCashFlowRuleLibrary));
            Assert.Equal(libraryId, repoInvocation.Arguments[0]);
        }


        [Fact]
        public async Task GetScenarioCashFlowRules_CallsGetScenarioCashFlowRulesOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();

            // Act
            var result = await controller.GetScenarioCashFlowRules(simulationId);

            // Assert
            ActionResultAssertions.OkObject(result);
            repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.GetScenarioCashFlowRules));
        }

        [Fact]
        public async Task UpsertCashFlowRuleLibrary_CallsUpsertLibraryAndRulesOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var controller = CreateController(unitOfWork);
            var ruleId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var dto = CashFlowRuleLibraryDtos.WithSingleRule(libraryId);
            var otherRule = CashFlowRuleDtos.Rule();
            var otherRules = new List<CashFlowRuleDTO> { otherRule };
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).Returns(otherRules);

            var sync = new PagingSyncModel<CashFlowRuleDTO>()
            {
                UpdateRows = new List<CashFlowRuleDTO>() { dto.CashFlowRules[0] }
            };
            var libraryRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>()
            {
                IsNewLibrary = false,
                Library = dto,
                SyncModel = sync
            };
            var libraryUser = new LibraryUserDTO()
            {
                UserId = Guid.NewGuid(),
                UserName = "testLibraryUser",
                AccessLevel = LibraryAccessLevel.Modify
            };
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(libraryId, libraryUser);
            repo.SetupGetLibraryAccess(dto.Id, libraryExists);

            // Act
            await controller.UpsertCashFlowRuleLibrary(libraryRequest);

            // Assert
            var libraryInvocation = repo.SingleInvocationWithName(nameof(ICashFlowRuleRepository.UpsertCashFlowRuleLibraryAndRules));
            Assert.Equal(dto, libraryInvocation.Arguments[0]);
        }
    }
}
