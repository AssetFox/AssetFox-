using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class SimulationPagingServiceTests
    {
        private SimulationPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new SimulationPagingService(unitOfWork.Object, unitOfWork.Object.SimulationRepo);
            return service;
        }

        [Fact]
        public void GetUserScenarioPage_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            repo.Setup(r => r.GetUserScenarios()).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
            };

            var result = pagingService.GetUserScenarioPage(request);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO>(),
                TotalItems = 0,
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        [Fact]
        public void GetSharedScenarioPage_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            repo.Setup(r => r.GetSharedScenarios(false, true)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
            };

            var result = pagingService.GetSharedScenarioPage(request, false, true);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO>(),
                TotalItems = 0,
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        [Fact]
        public void GetSharedScenarioPage_RowToDelete_DeletesRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var dto = SimulationDtos.Dto(scenarioId);
            var dtos = new List<SimulationDTO> { dto };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
                RowsForDeletion = new List<Guid> { scenarioId },
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                Page = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetUserScenarioPage(
                request);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO>(),
                TotalItems = 0,
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        [Fact]
        public void GetUserScenarioPage_RequestSecondPage_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto(scenarioId, "sim");
            var dto2Clone = SimulationDtos.Dto(scenarioId, "sim");
            var dtos = new List<SimulationDTO> { dto1, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 2,
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto2Clone, item);
        }

        [Fact]
        public void GetSharedScenarioPage_NumberOfRowsGoesBeyondPageSize_TruncatesReturnedList()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId1 = Guid.NewGuid();
            var simulationId2 = Guid.NewGuid();
            var simulation1 = SimulationDtos.Dto(simulationId1, "sim1");
            var simulation2 = SimulationDtos.Dto(simulationId2, "sim2");

            repo.Setup(r => r.GetSharedScenarios(false, false)).ReturnsList(simulation1, simulation2);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var upsertRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSharedScenarioPage(pagingRequest, false, false);
            Assert.Equal(2, result.TotalItems);
            var returnedSimulation = result.Items.Single();
            ObjectAssertions.Equivalent(simulation1, returnedSimulation);
        }

        [Fact]
        public void GetSharedScenarioPage2_NumberOfRowsGoesBeyondPageSize_TruncatesReturnedList()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId1 = Guid.NewGuid();
            var simulationId2 = Guid.NewGuid();
            var simulation1 = SimulationDtos.Dto(simulationId1, "sim1");
            var simulation2 = SimulationDtos.Dto(simulationId2, "sim2");
            repo.Setup(r => r.GetSharedScenarios(false, false)).ReturnsList(simulation1, simulation2);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                Page = 2,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSharedScenarioPage(pagingRequest, false, false);

            Assert.Equal(2, result.TotalItems);
            var returnedSimulation = result.Items.Single();
            ObjectAssertions.Equivalent(simulation2, returnedSimulation);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInName()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId1 = Guid.NewGuid();
            var simulationId2 = Guid.NewGuid();
            var simulation1 = SimulationDtos.Dto(simulationId1, "Apple");
            var simulation2 = SimulationDtos.Dto(simulationId2, "Banana");
            repo.Setup(r => r.GetUserScenarios()).ReturnsList(simulation1, simulation2);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                Page = 1,
                search = "baNANA",
            };

            var result = pagingService.GetUserScenarioPage(request);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO> { simulation2 },
                TotalItems = 1,
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        private void RunGetUserScenarioPage_Search_FindsDate(Action<SimulationDTO, DateTime> dtoModifier)
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId1 = Guid.NewGuid();
            var simulationId2 = Guid.NewGuid();
            var simulation1 = SimulationDtos.Dto(simulationId1, "simulation1");
            var simulation2 = SimulationDtos.Dto(simulationId2, "simulation2");
            var simulation2Clone = SimulationDtos.Dto(simulationId2, "simulation2");
            dtoModifier(simulation1, new DateTime(2022, 4, 1));
            dtoModifier(simulation2, new DateTime(2021, 4, 1));
            dtoModifier(simulation2Clone, new DateTime(2021, 4, 1));
            repo.Setup(r => r.GetUserScenarios()).ReturnsList(simulation1, simulation2);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                Page = 1,
                search = "2021",
            };

            var result = pagingService.GetUserScenarioPage(request);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO> { simulation2Clone },
                TotalItems = 1,
            };
            ObjectAssertions.Equivalent(expected, result);
        }
        private void RunGetUserScenarioPage_Search_FindsString(Action<SimulationDTO, string> dtoModifier)
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var simulationId1 = Guid.NewGuid();
            var simulationId2 = Guid.NewGuid();
            var simulation1 = SimulationDtos.Dto(simulationId1, "simulation1");
            var simulation2 = SimulationDtos.Dto(simulationId2, "simulation2");
            var simulation2Clone = SimulationDtos.Dto(simulationId2, "simulation2");
            dtoModifier(simulation1, "Apple");
            dtoModifier(simulation2, "Banana");
            dtoModifier(simulation2Clone, "Banana");
            repo.Setup(r => r.GetUserScenarios()).ReturnsList(simulation1, simulation2);
            var syncModel = new PagingSyncModel<SimulationDTO>
            {
            };
            var request = new PagingRequestModel<SimulationDTO>
            {
                Page = 1,
                search = "baNANA",
            };

            var result = pagingService.GetUserScenarioPage(request);

            var expected = new PagingPageModel<SimulationDTO>
            {
                Items = new List<SimulationDTO> { simulation2Clone },
                TotalItems = 1,
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInNetworkName()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.NetworkName = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInOwner()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.Owner = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInCreator()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.Creator = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInStatus()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.Status = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInReportStatus()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.ReportStatus = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInRuntime()
        {
            RunGetUserScenarioPage_Search_FindsString((dto, str) => dto.RunTime = str);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInLastRun()
        {
            RunGetUserScenarioPage_Search_FindsDate((dto, dateTime) => dto.LastRun = dateTime);
        }


        [Fact]
        public void GetUserScenarioPage_Search_FindsInCreatedDate()
        {
            RunGetUserScenarioPage_Search_FindsDate((dto, dateTime) => dto.CreatedDate = dateTime);
        }

        [Fact]
        public void GetUserScenarioPage_Search_FindsInLastModifiedDate()
        {
            RunGetUserScenarioPage_Search_FindsDate((dto, dateTime) => dto.LastModifiedDate = dateTime);
        }
    }
}
