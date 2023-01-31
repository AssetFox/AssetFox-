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

        [Fact] 
        public async Task DeleteSimulation_CallsDeleteOnRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Act
            var result = await controller.DeleteSimulationOperation(simulationId);
            // Assert
            ActionResultAssertions.Ok(result);
            var repoCall = repo.SingleInvocationWithName(nameof(ISimulationRepository.DeleteSimulation));
            Assert.Equal(simulationId, repoCall.Arguments[0]);
        }

        [Fact]
        public async Task GetUserScenariosPage_CallsGetUserScenariosOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulation = SimulationTestSetup.TestSimulation();
            var simulations = new List<SimulationDTO> { simulation };
            repo.Setup(r => r.GetUserScenarios()).Returns(simulations);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await controller.GetUserScenariosPage(request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var castValue = value as PagingPageModel<SimulationDTO>;
            ObjectAssertions.Equivalent(castValue.Items, simulations);
        }

        [Fact]
        public async Task GetSharedScenariosPage_CallsGetSharedScenariosOnRepo()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var simulation = SimulationTestSetup.TestSimulation();
            var simulations = new List<SimulationDTO> { simulation };
            repo.Setup(r => r.GetSharedScenarios(true, true)).Returns(simulations);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await controller.GetSharedScenariosPage(request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var castValue = value as PagingPageModel<SimulationDTO>;
            ObjectAssertions.Equivalent(simulations, castValue.Items);
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
