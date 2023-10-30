using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CommittedProjectPagingServiceTests
    {
        private CommittedProjectPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            return new CommittedProjectPagingService(unitOfWork.Object);
        }

        [Fact]
        public void GetCommittedProjectPage_PageSizeOne_ReturnsOneItem()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2);
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2 };
            var service = CreatePagingService(unitOfWork);

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 1,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);

            Assert.Equal(2, page.TotalItems);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            sectionCommittedProjects.Single(_ => _.Id == page.Items[0].Id);
        }

        [Fact]
        public void GetCommittedProjectPageSizeTwoSortColumnTreatmentIsDescendingFalse()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationDtos.Dto(simulationId);
            simulationRepo.SetupGetSimulation(simulation);
            var networkRepo = NetworkRepositoryMocks.New(unitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            networkRepo.SetupGetNetworkKeyAttribute(networkId);
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProjectId3 = Guid.NewGuid();
            var scenarioBudgetId1 = Guid.NewGuid();
            var scenarioBudgetId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioBudgetId1, simulationId);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2, scenarioBudgetId2, simulationId);
            var sectionCommittedProject3 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId3, scenarioBudgetId1, simulationId);
            sectionCommittedProject1.Treatment = "Treatmentz";
            sectionCommittedProject2.Treatment = "Treatmentx";
            sectionCommittedProject3.Treatment = "Treatmenty";
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2, sectionCommittedProject3 };
            var returnDictionary = new Dictionary<Guid, string>
            {
                {
                    scenarioBudgetId1, "Interstate"
                },
                {
                    scenarioBudgetId2, "Local"
                }
            };
            budgetRepo.Setup(br => br.GetScenarioBudgetDictionary(It.Is<List<Guid>>(
                list => list.Contains(scenarioBudgetId1) && list.Contains(scenarioBudgetId2)
                        && list.Count == 2))).Returns(returnDictionary);
            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = "treatment"
            };

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);
            Assert.Equal(3, page.TotalItems);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            Assert.Equal(sectionCommittedProjectId2, page.Items[0].Id);
            Assert.Equal(sectionCommittedProjectId3, page.Items[1].Id);
        }

        [Fact]
        public void GetCommittedProjectPageSizeTwoSortColumnTreatment_Nulls_DoesNotThrow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationDtos.Dto(simulationId);
            simulationRepo.SetupGetSimulation(simulation);
            var networkRepo = NetworkRepositoryMocks.New(unitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            networkRepo.SetupGetNetworkKeyAttribute(networkId);
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProjectId3 = Guid.NewGuid();
            var scenarioBudgetId1 = Guid.NewGuid();
            var scenarioBudgetId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioBudgetId1, simulationId);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2, scenarioBudgetId2, simulationId);
            var sectionCommittedProject3 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId3, scenarioBudgetId1, simulationId);
            sectionCommittedProject1.Treatment = null;
            sectionCommittedProject2.Treatment = "";
            sectionCommittedProject3.Treatment = "Treatmenty";
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2, sectionCommittedProject3 };
            var returnDictionary = new Dictionary<Guid, string>
            {
                {
                    scenarioBudgetId1, "Interstate"
                },
                {
                    scenarioBudgetId2, "Local"
                }
            };
            budgetRepo.Setup(br => br.GetScenarioBudgetDictionary(It.Is<List<Guid>>(
                list => list.Contains(scenarioBudgetId1) && list.Contains(scenarioBudgetId2)
                        && list.Count == 2))).Returns(returnDictionary);
            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = "treatment"
            };

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);
            Assert.Equal(3, page.TotalItems);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
        }

        [Fact]
        public void GetCommittedProjectPageSizeTwoSearch_Nulls_DoesNotThrow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationDtos.Dto(simulationId);
            simulationRepo.SetupGetSimulation(simulation);
            var networkRepo = NetworkRepositoryMocks.New(unitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            networkRepo.SetupGetNetworkKeyAttribute(networkId);
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProjectId3 = Guid.NewGuid();
            var scenarioBudgetId1 = Guid.NewGuid();
            var scenarioBudgetId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioBudgetId1, simulationId);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2, scenarioBudgetId2, simulationId);
            var sectionCommittedProject3 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId3, scenarioBudgetId1, simulationId);
            sectionCommittedProject1.Treatment = null;
            sectionCommittedProject3.LocationKeys = null;
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2, sectionCommittedProject3 };
            var returnDictionary = new Dictionary<Guid, string>
            {
                {
                    scenarioBudgetId1, "Interstate"
                },
                {
                    scenarioBudgetId2, "Local"
                }
            };
            budgetRepo.Setup(br => br.GetScenarioBudgetDictionary(It.Is<List<Guid>>(
                list => list.Contains(scenarioBudgetId1) && list.Contains(scenarioBudgetId2)
                        && list.Count == 2))).Returns(returnDictionary);
            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "won't find",
            };

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);
            Assert.Equal(3, page.TotalItems);
            Assert.Empty(page.Items);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoSearchItemsCountOne()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationDtos.Dto(simulationId);
            simulationRepo.SetupGetSimulation(simulation);
            var networkRepo = NetworkRepositoryMocks.New(unitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            networkRepo.SetupGetNetworkKeyAttribute(networkId);
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProjectId3 = Guid.NewGuid();
            var scenarioTreatmentId1 = Guid.NewGuid();
            var scenarioTreatmentId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioTreatmentId1, simulationId);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2, scenarioTreatmentId2, simulationId);
            var sectionCommittedProject3 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId3, scenarioTreatmentId1, simulationId);
            sectionCommittedProject1.Treatment = "Simple";
            sectionCommittedProject2.Treatment = "Complicated";
            sectionCommittedProject3.Treatment = "Simple";
            sectionCommittedProject1.LocationKeys[TestAttributeNames.BrKey] = "1";
            sectionCommittedProject2.LocationKeys[TestAttributeNames.BrKey] = "2";
            sectionCommittedProject3.LocationKeys[TestAttributeNames.BrKey] = "1";
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2, sectionCommittedProject3 };

            var service = CreatePagingService(unitOfWork);

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "Simple",
                sortColumn = ""
            };
            var dictionary = new Dictionary<Guid, string>
            {
                {
                    scenarioTreatmentId1, "Simple"
                },
                {
                    scenarioTreatmentId2, "Complicated"
                }
            };
            budgetRepo.Setup(b => b.GetScenarioBudgetDictionary(It.Is<List<Guid>>(list => list.Count == 2)))
                .Returns(dictionary);

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);

            Assert.Equal(3, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.True(page.Items.All(_ => _.Treatment == "Simple"));
        }

        [Fact]
        public void GetCommittedProjectPage_AddRow_TwoProjectsExist_ReturnsThree() 
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var service = CreatePagingService(unitOfWork);
            var dto1 = SectionCommittedProjectDtos.Dto();
            var dto2 = SectionCommittedProjectDtos.Dto();
            var dtos = new List<SectionCommittedProjectDTO> { dto1, dto2 };

            var addrow = new SectionCommittedProjectDTO()
            {
                Id = Guid.NewGuid(),
                Year = 2022,
                Treatment = "Something",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Cost = 10000,
                SimulationId = TestDataForCommittedProjects.SimulationId,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", TestDataForCommittedProjects.MaintainableAssetIdString1 },
                    { TestAttributeNames.BrKey, "1" },
                    { TestAttributeNames.BmsId, "12345678" }
                }
            };

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 4,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    AddedRows = new List<SectionCommittedProjectDTO> { addrow }
                },
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(dtos, request);

            Assert.Equal(3, page.TotalItems);
            Assert.Equal(3, page.Items.Count);
            Assert.True(page.Items.SingleOrDefault(_ => _.Id == addrow.Id) != null);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoUpdateRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationDtos.Dto(simulationId);
            simulationRepo.SetupGetSimulation(simulation);
            var networkRepo = NetworkRepositoryMocks.New(unitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            networkRepo.SetupGetNetworkKeyAttribute(networkId);
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId1 = Guid.NewGuid();
            var sectionCommittedProjectId2 = Guid.NewGuid();
            var sectionCommittedProjectId3 = Guid.NewGuid();
            var scenarioTreatmentId1 = Guid.NewGuid();
            var scenarioTreatmentId2 = Guid.NewGuid();
            var sectionCommittedProject1 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioTreatmentId1, simulationId);
            var sectionCommittedProject2 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId2, scenarioTreatmentId2, simulationId);
            var sectionCommittedProject3 = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId3, scenarioTreatmentId1, simulationId);
            var updateRow = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId1, scenarioTreatmentId1, simulationId);
            updateRow.Treatment = "updated treatment";
            sectionCommittedProject1.Treatment = "Simple";
            sectionCommittedProject2.Treatment = "Z Complicated";
            sectionCommittedProject3.Treatment = "Simple";
            sectionCommittedProject1.LocationKeys[TestAttributeNames.BrKey] = "1";
            sectionCommittedProject2.LocationKeys[TestAttributeNames.BrKey] = "2";
            sectionCommittedProject3.LocationKeys[TestAttributeNames.BrKey] = "1";
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO> { sectionCommittedProject1, sectionCommittedProject2, sectionCommittedProject3 };

            var newTreament = "updated treatment";
            updateRow.Treatment = newTreament;

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    UpdateRows = new List<SectionCommittedProjectDTO> { updateRow }
                },
                search = "",
                sortColumn = "treatment"
            };

            var page = service.GetCommittedProjectPage(sectionCommittedProjects, request);

            Assert.Equal(3, page.TotalItems);
            Assert.Equal(request.RowsPerPage, page.Items.Count);
            var updatedItem = page.Items.Single(_ => _.Id == sectionCommittedProjectId1);
            Assert.Equal(newTreament, updatedItem.Treatment);
        }

        [Fact]
        public void GetSyncedDataset_NoChanges_GrabsFromRepo()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var service = CreatePagingService(unitOfWork);
            var dto = SectionCommittedProjectDtos.Dto();
            var dtos = new List<SectionCommittedProjectDTO> { dto };
            var simulationId = Guid.NewGuid();
            var committedProjectRepo = CommittedProjectRepositoryMocks.New(unitOfWork);
            committedProjectRepo.Setup(c => c.GetSectionCommittedProjectDTOs(simulationId)).Returns(dtos);

            var dataSet = service.GetSyncedDataset(simulationId, new PagingSyncModel<SectionCommittedProjectDTO>());
            var actual = dataSet.Single();
            Assert.Equal(dto, actual);
        }

        [Fact]
        public void GetSyncedDataset_UpdateRowInRequest_Updates()
        {

            var unitOfWork = UnitOfWorkMocks.New();
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId = Guid.NewGuid();
            var scenarioBudgetId = Guid.NewGuid();
            var dto = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId, scenarioBudgetId);
            var dtos = new List<SectionCommittedProjectDTO> { dto };
            var simulationId = Guid.NewGuid();
            var committedProjectRepo = CommittedProjectRepositoryMocks.New(unitOfWork);
            committedProjectRepo.Setup(c => c.GetSectionCommittedProjectDTOs(simulationId)).Returns(dtos);
            var updateDto = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId, scenarioBudgetId);
            updateDto.Treatment = "Update me";
            updateDto.Cost = 1000000;
            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                UpdateRows = new List<SectionCommittedProjectDTO> { updateDto }
            };
            var dataSet = service.GetSyncedDataset(simulationId, sync);
            var actual = dataSet.Single();
            ObjectAssertions.Equivalent(updateDto, actual);
        }

        [Fact]
        public void GetSyncedData_AddRowInRequest_Adds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var service = CreatePagingService(unitOfWork);
            var sectionCommittedProjectId = Guid.NewGuid();
            var scenarioBudgetId = Guid.NewGuid();
            var dto = SectionCommittedProjectDtos.Dto(sectionCommittedProjectId, scenarioBudgetId);
            var dtos = new List<SectionCommittedProjectDTO> { dto };
            var simulationId = Guid.NewGuid();
            var committedProjectRepo = CommittedProjectRepositoryMocks.New(unitOfWork);
            committedProjectRepo.Setup(c => c.GetSectionCommittedProjectDTOs(simulationId)).Returns(dtos);
            var addDto = SectionCommittedProjectDtos.Dto();

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                AddedRows = new List<SectionCommittedProjectDTO> { addDto }
            };

            var dataSet = service.GetSyncedDataset(simulationId, sync);
            Assert.Equal(2, dataSet.Count);
            var unchanged = dataSet.Single(x => x.Id == sectionCommittedProjectId);
            var added = dataSet.Single(x => x.Id == addDto.Id);
            ObjectAssertions.Equivalent(dto, unchanged);
            ObjectAssertions.Equivalent(addDto, added);
        }
    }
}
