using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
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
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class RemainingLifeLimitTests
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

        public RemainingLifeLimitController SetupController()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new RemainingLifeLimitController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object,
                new RemainingLifeLimitPagingService(TestHelper.UnitOfWork));
            return controller;
        }

        public RemainingLifeLimitLibraryEntity TestRemainingLifeLimitLibrary(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new RemainingLifeLimitLibraryEntity
            {
                Id = resolveId,
                Name = "Test Name"
            };
            return returnValue;
        }

        public RemainingLifeLimitEntity TestRemainingLifeLimit(Guid libraryId, Guid attributeId)
        {
            return new RemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                RemainingLifeLimitLibraryId = libraryId,
                Value = 1.0,
                AttributeId = attributeId,
            };
        }

        public ScenarioRemainingLifeLimitEntity ScenarioTestRemainingLifeLimit(Guid scenarioId, Guid attributeId)
        {
            return new ScenarioRemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = scenarioId,
                Value = 1.0,
                AttributeId = attributeId,
            };
        }

        private RemainingLifeLimitLibraryEntity SetupForGet()
        {
            var library = TestRemainingLifeLimitLibrary();
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var lifeLimit = TestRemainingLifeLimit(library.Id, attribute.Id);
            TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Add(library);
            TestHelper.UnitOfWork.Context.RemainingLifeLimit.Add(lifeLimit);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return library;
        }

        private ScenarioRemainingLifeLimitEntity SetupForScenarioGet(Guid scenarioId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var lifeLimit = ScenarioTestRemainingLifeLimit(scenarioId, attribute.Id);
            TestHelper.UnitOfWork.Context.ScenarioRemainingLifeLimit.Add(lifeLimit);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return lifeLimit;
        }

        private CriterionLibraryDTO SetupForUpsertOrDelete()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            return criterionLibrary;
        }

        [Fact]
        public async Task RemainingLifeLimitLibraries_GetsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var libraryDto = RemainingLifeLimitLibraryDtos.Empty();
            var libraryDtos = new List<RemainingLifeLimitLibraryDTO> { libraryDto };
            repo.Setup(r => r.GetAllRemainingLifeLimitLibrariesNoChildren()).Returns(libraryDtos);
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.RemainingLifeLimitLibraries();

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(libraryDtos, value);
        }

        [Fact]
        public async Task UpsertRemainingLifeLimitLibrary_CallsUpsertLibraryAndUpsertLifeLimitsOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = RemainingLifeLimitRepositoryMocks.New(unitOfWork);
            var controller = CreateController(unitOfWork);

            var library = TestRemainingLifeLimitLibrary();
            var request = new LibraryUpsertPagingRequestModel<RemainingLifeLimitLibraryDTO, RemainingLifeLimitDTO>();
            request.IsNewLibrary = true;
            request.Library = library.ToDto();
            // Act
            var result = await controller
                .UpsertRemainingLifeLimitLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var repoLibraryCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertRemainingLifeLimitLibrary));
            var repoLimitCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertOrDeleteRemainingLifeLimits));
            ObjectAssertions.Equivalent(request.Library, repoLibraryCall.Arguments[0]);
            Assert.Equal(library.Id, repoLimitCall.Arguments[1]);
            var castArgumentZero = repoLimitCall.Arguments[0] as List<RemainingLifeLimitDTO>;
            Assert.Empty(castArgumentZero);
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
         //   var lifeLimitDto = RemainingLifeLimitDtos.New();
            var libraryId = Guid.NewGuid();
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
            // Act
            await controller.UpsertRemainingLifeLimitLibrary(libraryRequest);

            // Assert
            var libraryCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertRemainingLifeLimitLibrary));
            var limitCall = repo.SingleInvocationWithName(nameof(IRemainingLifeLimitRepository.UpsertOrDeleteRemainingLifeLimits));
            var modifiedDto = libraryCall.Arguments[0] as RemainingLifeLimitLibraryDTO;
            var modifiedLimits = limitCall.Arguments[0] as List<RemainingLifeLimitDTO>;

            Assert.Equal("Updated Description", modifiedDto.Description);
            Assert.Equal("attribute", modifiedLimits[0].Attribute);
        }

        [Fact]
        public async Task ShouldModifyScenarioRemainingLifeLimitData()
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
        public async Task ShouldGetLibraryRemainingLifeLimitPageData()
        {
            var controller = SetupController();
            // Arrange
            var library = SetupForGet().ToDto();
            var limit = library.RemainingLifeLimits[0];

            // Act
            var request = new PagingRequestModel<RemainingLifeLimitDTO>();
            var result = await controller.GetLibraryRemainingLifeLimitPage(library.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<RemainingLifeLimitDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<RemainingLifeLimitDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioRemainingLifeLimitPageData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var limit = SetupForScenarioGet(simulation.Id).ToDto();

            // Act
            var request = new PagingRequestModel<RemainingLifeLimitDTO>();
            var result = await controller.GetScenarioRemainingLifeLimitPage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<RemainingLifeLimitDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<RemainingLifeLimitDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(limit.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldDeleteRemainingLifeLimitData()
        {
            // Arrange
            var controller = SetupController();
            var library = SetupForGet();
            var criterionLibrary = SetupForUpsertOrDelete();
            var dtos = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();

            var remainingLifeLimitLibraryDTO = dtos.Single(lib => lib.Id == library.Id);
            var remainingLifeLimitDto = remainingLifeLimitLibraryDTO.RemainingLifeLimits[0];
            remainingLifeLimitDto.CriterionLibrary =
                criterionLibrary;

            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertRemainingLifeLimitLibrary(remainingLifeLimitLibraryDTO);

            // Act
            var result = await controller.DeleteRemainingLifeLimitLibrary(library.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.False(TestHelper.UnitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.RemainingLifeLimit.Any(_ => _.RemainingLifeLimitLibraryId == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibraryRemainingLifeLimit.Any(_ => _.RemainingLifeLimitId == remainingLifeLimitDto.Id));
        }
    }
}
