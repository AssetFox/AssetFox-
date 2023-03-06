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
    public class SimulationPagingServiceSortTests
    {
        private SimulationPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new SimulationPagingService(unitOfWork.Object, unitOfWork.Object.SimulationRepo);
            return service;
        }

        [Fact]
        public void GetUserScenarioPage_SortByName_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto(null, "A sim");
            var dto2 = SimulationDtos.Dto(null, "b sim");
            var dto3 = SimulationDtos.Dto(null, "C sim");
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "name",
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedNames = result.Items.Select(i => i.Name).ToList();
            var expected = new List<string> { "A sim", "b sim", "C sim" };
            ObjectAssertions.Equivalent(expected, returnedNames);
        }

        [Fact]
        public void GetUserScenarioPage_SortDescendingByName_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto(null, "A sim");
            var dto2 = SimulationDtos.Dto(null, "b sim");
            var dto3 = SimulationDtos.Dto(null, "C sim");
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "name",
                isDescending = true,
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedNames = result.Items.Select(i => i.Name).ToList();
            var expected = new List<string> { "C sim", "b sim", "A sim" };
            ObjectAssertions.Equivalent(expected, returnedNames);
        }

        [Fact]
        public void GetUserScenarioPage_SortByLastRun_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.LastRun = new DateTime(2020, 2, 21);
            dto2.LastRun = new DateTime(2021, 2, 21);
            dto3.LastRun = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "lastrun",
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.LastRun).ToList();
            var expected = new List<DateTime> { new DateTime(2020, 2, 21), new DateTime(2021, 2, 21), new DateTime(2022, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }


        [Fact]
        public void GetUserScenarioPage_SortDescendingByLastRun_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.LastRun = new DateTime(2020, 2, 21);
            dto2.LastRun = new DateTime(2021, 2, 21);
            dto3.LastRun = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "lastrun",
                isDescending = true,
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.LastRun).ToList();
            var expected = new List<DateTime> { new DateTime(2022, 2, 21), new DateTime(2021, 2, 21), new DateTime(2020, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }

        [Fact]
        public void GetUserScenarioPage_SortByCreateDate_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.CreatedDate = new DateTime(2020, 2, 21);
            dto2.CreatedDate = new DateTime(2021, 2, 21);
            dto3.CreatedDate = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "CREATEDDATE",
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.CreatedDate).ToList();
            var expected = new List<DateTime> { new DateTime(2020, 2, 21), new DateTime(2021, 2, 21), new DateTime(2022, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }

        [Fact]
        public void GetUserScenarioPage_SortDescendingByCreateDate_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.CreatedDate = new DateTime(2020, 2, 21);
            dto2.CreatedDate = new DateTime(2021, 2, 21);
            dto3.CreatedDate = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "CREATEDDATE",
                isDescending = true,
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.CreatedDate).ToList();
            var expected = new List<DateTime> { new DateTime(2022, 2, 21), new DateTime(2021, 2, 21), new DateTime(2020, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }

        [Fact]
        public void GetUserScenarioPage_SortByLastModifiedDate_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.LastModifiedDate = new DateTime(2020, 2, 21);
            dto2.LastModifiedDate = new DateTime(2021, 2, 21);
            dto3.LastModifiedDate = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "lastmodifieddate",
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.LastModifiedDate).ToList();
            var expected = new List<DateTime> { new DateTime(2020, 2, 21), new DateTime(2021, 2, 21), new DateTime(2022, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }

        [Fact]
        public void GetUserScenarioPage_SortDescendingByLastModifiedDate_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var dto1 = SimulationDtos.Dto();
            var dto2 = SimulationDtos.Dto();
            var dto3 = SimulationDtos.Dto();
            dto1.LastModifiedDate = new DateTime(2020, 2, 21);
            dto2.LastModifiedDate = new DateTime(2021, 2, 21);
            dto3.LastModifiedDate = new DateTime(2022, 2, 21);
            var dtos = new List<SimulationDTO> { dto1, dto3, dto2 };
            repo.Setup(r => r.GetUserScenarios()).Returns(dtos);
            var syncModel = new PagingSyncModel<SimulationDTO>();
            var pagingRequest = new PagingRequestModel<SimulationDTO>
            {
                SyncModel = syncModel,
                sortColumn = "lastmodifieddate",
                isDescending = true,
            };

            var result = pagingService.GetUserScenarioPage(pagingRequest);

            var returnedDates = result.Items.Select(i => i.LastModifiedDate).ToList();
            var expected = new List<DateTime> { new DateTime(2022, 2, 21), new DateTime(2021, 2, 21), new DateTime(2020, 2, 21) };
            ObjectAssertions.Equivalent(expected, returnedDates);
        }
    }
}
