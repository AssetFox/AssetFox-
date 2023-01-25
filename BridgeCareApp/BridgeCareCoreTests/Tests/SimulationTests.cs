using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class SimulationTests
    {
        private SimulationController _controller;

        private UserEntity _testUserEntity;
        private SimulationEntity _testSimulationToClone;
        private const string SimulationName = "Simulation";
        private static readonly Guid UserId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();
        private readonly Mock<ISimulationQueueService> _mockSimulationQueueService = new();

        private SimulationController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var claimHelper = ClaimHelperMocks.New();
            var simulationAnalysis = SimulationAnalysisMocks.New();
            var pagingSerivce = new SimulationPagingService(unitOfWork.Object, unitOfWork.Object.SimulationRepo);
            var queueService = SimulationQueueServiceMocks.New();
            var controller = new SimulationController(
                simulationAnalysis.Object,
                pagingSerivce,
                queueService.Object,
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                claimHelper.Object
                );
            return controller;
        }

        public SimulationAnalysisService Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(TestHelper.UnitOfWork);

            var simulationAnalysisService =
                new SimulationAnalysisService(TestHelper.UnitOfWork, new());
            return simulationAnalysisService;
        }

        private void CreateAuthorizedController(SimulationAnalysisService simulationAnalysisService)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new SimulationController(
                simulationAnalysisService,
                new SimulationPagingService(TestHelper.UnitOfWork, new SimulationRepository(TestHelper.UnitOfWork)),
                _mockSimulationQueueService.Object,
                EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object);
        }

        [Fact] // Seems to be some connection with other tests here. For example, WJ had a failure in an "unrelated" attribute import test that fried it.
        public async Task ShouldDeleteSimulation()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            await _controller.DeleteSimulationOperation(simulation.Id);
            // Assert
            Assert.True(!TestHelper.UnitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id));
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGetUserScenariosPage()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await _controller.GetUserScenariosPage(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGetSharedScenariosPage()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await _controller.GetSharedScenariosPage(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.TestSimulation();
            var result = await _controller.CreateSimulation(NetworkTestSetup.NetworkId, simulation);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPut()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            // Act
            var result = await _controller.UpdateSimulation(simulation);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);

            // Act
            var result = await _controller.DeleteSimulationOperation(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetSimulationCreatedByUser()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, owner:TestHelper.UnitOfWork.CurrentUser.Id);
            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var userResult = await _controller.GetUserScenariosPage(request);

            // Assert
            var okObjResult = userResult as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = ((PagingPageModel<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(PagingPageModel<SimulationDTO>))).Items;
            var dto = dtos.Single(dto => dto.Id == simulation.Id);
        }

        [Fact]
        public async Task ShouldGetSimulationSharedWithUser()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 100,
                search = "",
                sortColumn = ""
            };

            // Act
            var sharedResult = await _controller.GetSharedScenariosPage(request);

            // Assert
            var okObjResult = sharedResult as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = ((PagingPageModel<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(PagingPageModel<SimulationDTO>))).Items;
            Assert.NotEmpty(dtos);
            var dtoFromThisTest = dtos.Single(dto => dto.Id == simulation.Id);
            Assert.True(dtos.All(_ => _.Owner != TestHelper.UnitOfWork.CurrentUser.Username));
        }

        [Fact]
        public async Task CreateSimulation_CallsCreateOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var networkId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var newSimulationDto = SimulationTestSetup.TestSimulation(simulationId);
            var simulationDtoAfter = SimulationTestSetup.TestSimulation(simulationId, newSimulationDto.Name);
            var userId = Guid.NewGuid();
            var simulationUserDto = SimulationUserDtos.Dto(userId);

            newSimulationDto.Users = new List<SimulationUserDTO>
                {
                    simulationUserDto,
                };
            simulationDtoAfter.Users = new List<SimulationUserDTO> { simulationUserDto };
            repo.Setup(r => r.GetSimulation(newSimulationDto.Id)).Returns(simulationDtoAfter);

            // Act
            var result =
                await controller.CreateSimulation(networkId, newSimulationDto) as OkObjectResult;
            var dto = (SimulationDTO)Convert.ChangeType(result!.Value, typeof(SimulationDTO));

            // Assert
            var createCall = repo.SingleInvocationWithName(nameof(ISimulationRepository.CreateSimulation));
            Assert.Equal(networkId, createCall.Arguments[0]);
            Assert.Equal(newSimulationDto, createCall.Arguments[1]);
            ObjectAssertions.Equivalent(dto, newSimulationDto);
        }

        [Fact]
        public async Task UpdateSimulation_CallsUpdateOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var userId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var simulationDTO = SimulationTestSetup.TestSimulation(simulationId, SimulationName, userId);
            var simulationDTO2 = SimulationTestSetup.TestSimulation(simulationId, SimulationName, userId);
            var simulationDTO3 = SimulationTestSetup.TestSimulation(simulationId, SimulationName, userId);

            repo.Setup(r => r.GetSimulation(simulationId)).Returns(simulationDTO2);

            // Act
            var result = await controller.UpdateSimulation(simulationDTO);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            ObjectAssertions.Equivalent(simulationDTO3, value);
            var repoCall = repo.SingleInvocationWithName(nameof(ISimulationRepository.UpdateSimulation));
            ObjectAssertions.Equivalent(simulationDTO3, repoCall.Arguments[0]);
        }

        [Fact]
        public async Task CloneSimulation_CallsCloneSimulationOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulationId = Guid.NewGuid();
            var networkId = Guid.NewGuid();
            var cloneSimulationId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var simulationDto = SimulationTestSetup.TestSimulation(cloneSimulationId, SimulationName, ownerId);
            var cloneResult = new SimulationCloningResultDTO
            {
                Simulation = simulationDto,
                WarningMessage = null,
            };
            repo.Setup(r => r.CloneSimulation(simulationId, networkId, SimulationName)).Returns(cloneResult);
            var cloneSimulationDto = new CloneSimulationDTO
            {
                scenarioId = simulationId,
                networkId = networkId,
                scenarioName = SimulationName,
            };

            var result = await controller.CloneSimulation(cloneSimulationDto);

            var value = ActionResultAssertions.OkObject(result);
            Assert.Equal(simulationDto, value);
        }
    }
}
