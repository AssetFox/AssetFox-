using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class RemainingLifeLimitControllerTests
    {
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private RemainingLifeLimitController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var deficientConditionGoalService = new RemainingLifeLimitPagingService(unitOfWork.Object);
            var controller = new RemainingLifeLimitController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                claimHelper.Object,
                deficientConditionGoalService
                );
            return controller;
        }

        [Fact]
        public async Task UpsertRemainingLifeLimitLibrary_CallsUpsertLibraryAndUpsertLifeLimitsOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);

            var libraryId = Guid.NewGuid();
            var dto = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var dtoClone = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var request = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>();
            request.IsNewLibrary = true;
            request.Library = dto;

            var libraryDoesNotExist = LibraryAccessModels.LibraryDoesNotExist();
            repo.SetupGetLibraryAccess(dto.Id, libraryDoesNotExist);

            // Act
            var result = await controller
                .UpsertRemainingLifeLimitLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var repoLibraryCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertRemainingLifeLimitLibraryAndLimits));
            ObjectAssertions.Equivalent(dtoClone, repoLibraryCall.Arguments[0]);
        }

        [Fact]
        public async Task DeleteRemainingLifeLimitLibrary_CallsThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);

            // Act
            var libraryId = Guid.NewGuid();
            var result = await controller.DeleteRemainingLifeLimitLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var repoDeleteCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.DeleteRemainingLifeLimitLibrary));
            Assert.Equal(libraryId, repoDeleteCall.Arguments[0]);
        }

        [Fact]
        public async Task RemainingLifeLimitLibraries_CallsGetAllRemainingLifeLimitLibrariesNoChildren()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = RemainingLifeLimitLibraryDtos.Empty();
            var dtos = new List<RemainingLifeLimitLibraryDTO> { dto };
            repo.Setup(r => r.GetAllRemainingLifeLimitLibrariesNoChildren()).Returns(dtos);

            // Act
            var result = await controller.RemainingLifeLimitLibraries();

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var actualDtos = (List<RemainingLifeLimitLibraryDTO>)Convert.ChangeType(value,
                typeof(List<RemainingLifeLimitLibraryDTO>));
            var actualDto = actualDtos.Single(x => x.Id == dto.Id);
            ObjectAssertions.Equivalent(dto, actualDto);
        }

        [Fact]
        public async Task UpsertRemainingLifeLimitLibrary_UpdateRowInRequest_CallsUpdateOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            var user = UserDtos.Admin();
            var dto = RemainingLifeLimitLibraryDtos.Empty(libraryId);
            var remainingLifeLimitId = Guid.NewGuid();
            var remainingLifeLimit = RemainingLifeLimitDtos.Dto("attribute", remainingLifeLimitId);
            var remainingLifeLimit2 = RemainingLifeLimitDtos.Dto("attribute", remainingLifeLimitId);
            dto.Description = "Updated Description";
            remainingLifeLimit.Value = 2.0;
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            remainingLifeLimit.CriterionLibrary =
                criterionLibrary;
            repo.Setup(r => r.GetRemainingLifeLimitsByLibraryId(libraryId)).Returns(new List<RemainingLifeLimitDTO> { remainingLifeLimit2 });
            var sync = new PagingSyncModel<RemainingLifeLimitDTO>()
            {
                UpdateRows = new List<RemainingLifeLimitDTO>() { remainingLifeLimit }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>()
            {
                IsNewLibrary = false,
                Library = dto,
                SyncModel = sync
            };
            var libraryUser = LibraryUserDtos.Modify(user.Id);
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(user.Id, libraryUser);
            repo.SetupGetLibraryAccess(libraryId, libraryExists);

            // Act
            await controller.UpsertRemainingLifeLimitLibrary(libraryRequest);

            // Assert
            var libraryCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertRemainingLifeLimitLibraryAndLimits));
            var modifiedDto = libraryCall.Arguments[0] as RemainingLifeLimitLibraryDTO;
            var modifiedLimits = modifiedDto.RemainingLifeLimits;

            Assert.Equal("Updated Description", modifiedDto.Description);
            Assert.Equal("attribute", modifiedLimits[0].Attribute);
        }

        [Fact]
        public async Task UpsertScenarioRemainingLifeLimits_ModifiedLifeLimitInSyncModel_AsksRepoToUpdate()
        {
            // GetScenarioLifeLimits returns a singleton. Our sync model modifies it.
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var dto = RemainingLifeLimitDtos.Dto("attribute", limitId);
            var dto2 = RemainingLifeLimitDtos.Dto("attribute", limitId);
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(simulationId)).Returns(new List<RemainingLifeLimitDTO> { dto2 });
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            dto.Value = 3.0;
            dto.CriterionLibrary =
                criterionLibrary;
            var sync = new PagingSyncModel<RemainingLifeLimitDTO>()
            {
                UpdateRows = new List<RemainingLifeLimitDTO>() { dto }
            };

            // Act
            await controller.UpsertScenarioRemainingLifeLimits(simulationId, sync);

            // Assert
            var repoCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertOrDeleteScenarioRemainingLifeLimits));
            Assert.Equal(simulationId, repoCall.Arguments[1]);
            var upsertedLimits = repoCall.Arguments[0] as List<RemainingLifeLimitDTO>;
            var upsertedLimit = upsertedLimits.Single();
            Assert.Equal(3.0, upsertedLimit.Value);
        }

        [Fact]
        public async Task GetRemainingLifeLimitPage_CallsGetRemainingLifeLimitsByLibraryId()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();
            var limitId = Guid.NewGuid();
            var limit = RemainingLifeLimitDtos.Dto("attribute", limitId);
            var limits = new List<RemainingLifeLimitDTO> { limit };

            repo.Setup(r => r.GetRemainingLifeLimitsByLibraryId(libraryId)).Returns(limits);
            // Act
            var request = new PagingRequestModel<RemainingLifeLimitDTO>();
            var result = await controller.GetLibraryRemainingLifeLimitPage(libraryId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);

            var page = value
                as PagingPageModel<RemainingLifeLimitDTO>;
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public async Task GetScenarioRemainingLifeLimitPage_CallsGetScenarioRemainingLifeLimitsOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var limit = RemainingLifeLimitDtos.Dto("attribute");
            var limits = new List<RemainingLifeLimitDTO> { limit };
            repo.Setup(r => r.GetScenarioRemainingLifeLimits(simulationId)).Returns(limits);

            // Act
            var request = new PagingRequestModel<RemainingLifeLimitDTO>();
            var result = await controller.GetScenarioRemainingLifeLimitPage(simulationId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);

            var page = value as PagingPageModel<RemainingLifeLimitDTO>;
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public async Task DeleteRemainingLifeLimitLibrary_PassesThroughToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);
            var libraryId = Guid.NewGuid();

            // Act
            var result = await controller.DeleteRemainingLifeLimitLibrary(libraryId);

            // Assert
            var invocation = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.DeleteRemainingLifeLimitLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
        }
    }
}
