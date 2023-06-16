using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.General_Work_Queue;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace BridgeCareCoreTests.Tests
{
    public class PerformanceCurveControllerTests
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();


        private PerformanceCurveController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var expressionValidationService = new Mock<IExpressionValidationService>();
            var performanceCurvesService = new PerformanceCurvesService(unitOfWork.Object, hubService.Object, expressionValidationService.Object);
            var performanceCurvesPagingService = new PerformanceCurvesPagingService(unitOfWork.Object);
            var claimHelper = new Mock<IClaimHelper>();
            var generalWorkQueue = GeneralWorkQueueServiceMocks.New();
            var controller = new PerformanceCurveController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                performanceCurvesService,
                performanceCurvesPagingService,
                claimHelper.Object,
                generalWorkQueue.Object
                );
            return controller;
        }

        [Fact]
        public async Task GetPerformanceCurveLibraries_PassesToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var library = PerformanceCurveLibraryDtos.Empty();
            var libraries = new List<PerformanceCurveLibraryDTO> { library };
            repo.Setup(r => r.GetPerformanceCurveLibrariesNoPerformanceCurves()).Returns(libraries);
            // Arrange
            var controller = CreateController(unitOfWork);

            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(libraries, value);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_NewLibrary_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto2 = PerformanceCurveLibraryDtos.Empty(libraryId);

            var libraryDoesNotExist = LibraryAccessModels.LibraryDoesNotExist();
            repo.SetupGetLibraryAccess(libraryId, libraryDoesNotExist);

            var controller = CreateController(unitOfWork);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = dto,
                IsNewLibrary = true,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurveLibraryAndCurves));
            var argument = libraryCall.Arguments[0] as PerformanceCurveLibraryDTO;
            ObjectAssertions.Equivalent(dto2, argument);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibrary_NotNewLibrary_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var libraryUser = new LibraryUserDTO()
            {
                UserId = libraryId,
                AccessLevel = LibraryAccessLevel.Modify,
                UserName = "testLibraryUser"
            };

            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(libraryId, libraryUser);
            repo.SetupGetLibraryAccess(libraryId, libraryExists);
            var dto = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto2 = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto3 = PerformanceCurveLibraryDtos.Empty(libraryId);
            dto.Name = "Updated name";
            dto2.Name = "Updated name";
            repo.Setup(r => r.GetPerformanceCurveLibrary(libraryId)).Returns(dto3);
            repo.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());
            var controller = CreateController(unitOfWork);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = dto,
                IsNewLibrary = false,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurveLibraryAndCurves));
            ObjectAssertions.Equivalent(dto2, libraryCall.Arguments[0]);
        }

        [Fact]
        public async Task DeletePerformanceCurveLibrary_PassesRequestToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Act
            var result = await controller.DeletePerformanceCurveLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);
            var invocation = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.DeletePerformanceCurveLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationExists_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Arrange
            var simulationId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioPerformanceCurvesOrderedById(simulationId)).Returns(new List<PerformanceCurveDTO>());
           
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                },
                search = "",
                sortColumn = ""
            };
            // Act
            var result = await controller.GetScenarioPerformanceCurvePage(simulationId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var castValue = value as PagingPageModel<PerformanceCurveDTO>;
            Assert.Empty(castValue.Items);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_SimulationExists_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Arrange
            var performanceCurveId = Guid.NewGuid();
            var dto = PerformanceCurveDtos.Dto(performanceCurveId, criterionLibraryId);
            repo.Setup(r => r.GetScenarioPerformanceCurvesOrderedById(simulationId)).Returns(new List<PerformanceCurveDTO>());
            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                LibraryId = null,
                AddedRows = new List<PerformanceCurveDTO> { dto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };
            // Act
            var result = await controller
                .UpsertScenarioPerformanceCurves(simulationId,
                    request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Upsert_AsksRepositoryToUpsertCurvesAndLibrary()
        {
            var repositoryMock = new Mock<IPerformanceCurveRepository>();
            var user = UserDtos.Admin();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var userRepositoryMock = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var esecSecurity = EsecSecurityMocks.Admin;
            var pagingService = new Mock<IPerformanceCurvesPagingService>();
            var service = new Mock<IPerformanceCurvesService>();
            var performanceCurves = new List<PerformanceCurveDTO>();
            var libraryId = Guid.NewGuid();
            var libraryUser = new LibraryUserDTO()
            {
                UserId = libraryId,
                AccessLevel = LibraryAccessLevel.Modify,
                UserName = "testLibraryUser"
            };

            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(libraryId, libraryUser);
            repositoryMock.SetupGetLibraryAccess(libraryId, libraryExists);

            var pagingSync = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
            };
            var library = new PerformanceCurveLibraryDTO
            {
                Id = libraryId,
            };
            var pagingRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                SyncModel = pagingSync,
                Library = library,
            };
            pagingService.Setup(s => s.GetSyncedLibraryDataset(pagingRequest)).Returns(performanceCurves);
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repositoryMock.Object);
            var controller = PerformanceCurveControllerTestSetup.Create(
                esecSecurity,
                unitOfWork.Object,
                service.Object,
                pagingService.Object);
            

            await controller.UpsertPerformanceCurveLibrary(pagingRequest);

            repositoryMock.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurveLibraryAndCurves));
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_Gets()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var curve = PerformanceCurveDtos.Dto();
            var curves = new List<PerformanceCurveDTO> { curve };
            repo.Setup(r => r.GetScenarioPerformanceCurvesOrderedById(simulationId)).Returns(curves);
            var controller = CreateController(unitOfWork);
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                },
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };
            // Act
            var result = await controller.GetScenarioPerformanceCurvePage(simulationId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var page = (PagingPageModel<PerformanceCurveDTO>)Convert.ChangeType(value,
                typeof(PagingPageModel<PerformanceCurveDTO>));
            var actual = page.Items.Single();
            ObjectAssertions.Equivalent(curve, actual);
        }
    }
}
