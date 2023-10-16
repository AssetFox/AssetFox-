using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Treatment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class TreatmentTests
    {
        [Fact]
        public async Task ShouldGetSelectedTreatmentByIdWithData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var treatmentId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var library = TreatmentLibraryDtos.WithSingleTreatment(libraryId, treatmentId);
            treatmentRepo.Setup(t => t.GetTreatmentLibraryWithSingleTreatmentByTreatmentId(treatmentId)).Returns(library);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);

            // Act
            var result = await controller.GetSelectedTreatmentById(treatmentId);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (TreatmentDTO)Convert.ChangeType(okObjResult.Value,
                typeof(TreatmentDTO));
            ObjectAssertions.Equivalent(library.Treatments[0], dto);
        }

        [Fact]
        public async Task ShouldGetScenarioSelectedTreatmentByIdWithData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var treatmentId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var treatment = TreatmentDtos.Dto(treatmentId);
            var treatmentWithSimulation = new TreatmentDTOWithSimulationId
            {
                SimulationId = simulationId,
                Treatment = treatment,
            };
            treatmentRepo.Setup(t => t.GetScenarioSelectableTreatmentById(treatmentId)).Returns(treatmentWithSimulation);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);

            // Act
            var result = await controller.GetScenarioSelectedTreatmentById(treatmentId);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(treatment, value);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var dtos = new List<TreatmentDTO> { dto };
            treatmentRepo.Setup(tr => tr.GetScenarioSelectableTreatments(simulationId)).Returns(dtos);

            var result = await controller.GetScenarioSelectedTreatments(simulationId);

            // Assert
            var value = (result as OkObjectResult).Value;
            var actualId = (value as List<TreatmentDTO>).Single().Id;
            Assert.Equal(treatmentId, actualId);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);            
            var treatementLibraryUserRepo = TreatmentLibraryUserMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var pagingService = TreatmentPagingServiceMocks.EmptyMock;
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };
            var user = UserDtos.Admin();
            var libraryUser = LibraryUserDtos.Modify(user.Id);
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(user.Id, libraryUser);
            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = false,
                Library = dto,
            };
            treatementLibraryUserRepo.SetupGetLibraryAccess(libraryRequest.Library.Id, libraryExists);
            pagingService.Setup(ts => ts.GetSyncedLibraryDataset(It.IsAny<LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>>())).Returns(new List<TreatmentDTO>()); // correct? Merge build error here.
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService, pagingService);
            // Act
            var result = await controller.UpsertTreatmentLibrary(libraryRequest);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var pagingService = TreatmentPagingServiceMocks.EmptyMock;
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = true,
                Library = dto,
            };
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService);
            var dtos = new List<TreatmentDTO>();
            var simulation = new SimulationDTO { Id = simulationId };

            var pageSync = new PagingSyncModel<TreatmentDTO>();
            pagingService.Setup(ts => ts.GetSyncedScenarioDataSet(simulationId, pageSync)).Returns(dtos);

            // Act
            var result = await controller.UpsertScenarioSelectedTreatments(simulationId, pageSync);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryDelete()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var libraryId = Guid.NewGuid();

            // Act
            var result = await controller.DeleteTreatmentLibrary(libraryId);

            // Assert
            ActionResultAssertions.Ok(result);
            var deleteInvocation = treatmentRepo.SingleInvocationWithName(nameof(ISelectableTreatmentRepository.DeleteTreatmentLibrary));
            Assert.Equal(libraryId, deleteInvocation.Arguments[0]);
        }

        [Fact]
        public async Task ShouldGetLibraryTreatmentData()
        {
            // Arrange

            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = new TreatmentLibraryDTO
            {
                Id = libraryId,
            };
            var dtos = new List<TreatmentLibraryDTO> { dto };
            treatmentRepo.Setup(tr => tr.GetAllTreatmentLibrariesNoChildren()).Returns(dtos);

            // Act
            var result = await controller.GetTreatmentLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);
            var actualDtos = okObjResult.Value;
            Assert.Equal(dtos, actualDtos);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var treatment = new TreatmentDTO
            {
                Id = Guid.NewGuid(),
            };
            var expectedResult = new List<TreatmentDTO> { treatment };
            var simulationId = Guid.NewGuid();
            treatmentRepo.Setup(tr => tr.GetScenarioSelectableTreatments(simulationId)).Returns(expectedResult);

            // Act
            var result = await controller.GetScenarioSelectedTreatments(simulationId);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okObjResult.Value);
        }

        [Fact]
        public async Task ShouldModifyLibraryTreatmentData()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);            
            var treatmentLibraryRepo = TreatmentLibraryUserMocks.New(unitOfWork);
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var pagingService = TreatmentPagingServiceMocks.EmptyMock;            
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentBefore = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var treatmentAfter = new TreatmentDTO
            {
                Id = treatmentId,
                Description = "Updated description",
            };
            var treatmentsBefore = new List<TreatmentDTO> { treatmentBefore };
            var treatmentsAfter = new List<TreatmentDTO> { treatmentAfter };
            var libraryBefore = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsBefore,
            };
            var libraryAfter = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsAfter,
            };
 
            var sync = new PagingSyncModel<TreatmentDTO>()
            {
                UpdateRows = new List<TreatmentDTO>() { treatmentAfter },
                LibraryId = libraryId,
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = false,
                Library = libraryBefore,
                SyncModel = sync
            };
            pagingService.Setup(ts => ts.GetSyncedLibraryDataset(libraryRequest)).Returns(treatmentsAfter);
            var user = UserDtos.Admin();
            var libraryUser = LibraryUserDtos.Modify(user.Id);
            var libraryExists = LibraryAccessModels.LibraryExistsWithUsers(user.Id, libraryUser);
            treatmentLibraryRepo.SetupGetLibraryAccess(libraryId, libraryExists);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService, pagingService);

            // Act
            var result = await controller.UpsertTreatmentLibrary(libraryRequest);

            // Assert
            var libraryInvocation = treatmentLibraryRepo.SingleInvocationWithName(nameof(ITreatmentLibraryUserRepository.UpsertTreatmentLibraryUser));
            ObjectAssertions.Equivalent(libraryAfter, libraryInvocation.Arguments[0]);
            var libraryArgument = libraryInvocation.Arguments[0] as TreatmentLibraryDTO;
            Assert.Equal(treatmentsAfter, libraryArgument.Treatments);
        }

        [Fact]
        public async Task ShouldModifyScenarioTreatmentData()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var pagingService = TreatmentPagingServiceMocks.EmptyMock;
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService, pagingService);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentBefore = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var treatmentAfter = new TreatmentDTO
            {
                Id = treatmentId,
                Description = "Updated description",
            };
            var treatmentsBefore = new List<TreatmentDTO> { treatmentBefore };
            var treatmentsAfter = new List<TreatmentDTO> { treatmentAfter };
            var libraryBefore = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsBefore,
            };
            var libraryAfter = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsAfter,
            };

            var sync = new PagingSyncModel<TreatmentDTO>()
            {
                UpdateRows = new List<TreatmentDTO>() { treatmentAfter },
                LibraryId = libraryId,
            };
            pagingService.Setup(ts => ts.GetSyncedScenarioDataSet(simulationId, sync)).Returns(treatmentsAfter);

            var result = await controller.UpsertScenarioSelectedTreatments(simulationId, sync);
            ActionResultAssertions.Ok(result);
            var call = pagingService.SingleInvocationWithName(nameof(ITreatmentPagingService.GetSyncedScenarioDataSet));
            Assert.Equal(simulationId, call.Arguments[0]);
            Assert.Equal(sync, call.Arguments[1]);
        }
    }
}
